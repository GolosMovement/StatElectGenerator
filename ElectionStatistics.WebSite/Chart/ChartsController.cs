﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectionStatistics.Model;
using Microsoft.AspNetCore.Mvc;

namespace ElectionStatistics.WebSite
{
	[Route("api/charts")]
	public class ChartsController : Controller
	{
		private readonly ModelContext modelContext;

		public ChartsController(ModelContext modelContext)
		{
			this.modelContext = modelContext;
		}

		[HttpGet, Route("histogram")]
		public HighchartsOptions GetDataForHistogram(HistogramBuildParameters parameters)
		{
			var results = parameters.GetElectionResults(modelContext);
			var candidate = modelContext.Candidates.GetById(parameters.CandidateId);

			var data = results
				.Join(
					modelContext.ElectionCandidatesVotes.ByCandidate(candidate),
					result => result.Id,
					votes => votes.ElectionResultId,
					(result, votes) => new
					{
						VotersCount = result.InsideBallotsCount + result.OutsideBallotsCount,
						Value = result.InsideBallotsCount + result.OutsideBallotsCount == 0
							? 0
							: (double) votes.Count * 100 / (result.InsideBallotsCount + result.OutsideBallotsCount)
					})
				.GroupBy(arg => Math.Round(arg.Value / parameters.StepSize) * parameters.StepSize)
				.Select(grouping => new []
				{
					grouping.Key,
					grouping.Sum(arg => arg.VotersCount)
				})
				.ToArray();

			return new HighchartsOptions
			{
				XAxis = new AxisOptions
				{
					Min = 0,
					Max = 100,
					Title = new TitleOptions
					{
						Text = "% за кандидата"
					}
				},
				YAxis = new AxisOptions
				{
					Min = 0,
					Title = new TitleOptions
					{
						Text = "Количество избирателей зарегистрированных на участках"
					}
				},
				Series = new ChartSeries[]
				{
					new HistogramChartSeries
					{
						Name = candidate.ShortName,
						Data = data
					}
				}
			};
		}

		[HttpGet, Route("scatterplot")]
		public HighchartsOptions GetDataForScatterplot(ChartBuildParameters parameters)
		{
			var results = parameters.GetElectionResults(modelContext);
			var candidate = modelContext.Candidates.GetById(parameters.CandidateId);
			var highestDistricts = parameters.GetHighestDistricts(modelContext).ToArray();

			var sourceData = results
				.Join(
					modelContext.ElectionCandidatesVotes.ByCandidate(candidate),
					result => result.Id,
					votes => votes.ElectionResultId,
					(result, votes) => new
					{
						result.ElectoralDistrict.Id,
						DistrictName = result.ElectoralDistrict.Name,
						Value = result.InsideBallotsCount + result.OutsideBallotsCount == 0
							? 0
							: (double)votes.Count * 100 / (result.InsideBallotsCount + result.OutsideBallotsCount),
						result.ElectoralDistrict.HierarchyPath
					})
				.ToArray()
				.Select(arg => new
				{
					Number = int.Parse(arg.DistrictName.Replace("УИК №", "")),
					arg.DistrictName,
					arg.Value,
					HighestDistrict = highestDistricts.Single(
						district =>
							arg.HierarchyPath.StartsWith(district.GetChildrenHierarchyPath()))
				})
				.ToArray();

			var districtOrderNumbers = sourceData
				.GroupBy(arg => arg.HighestDistrict)
				.ToDictionary(grouping => grouping.Key, grouping => grouping.Min(arg => arg.Number));

			var series = sourceData
				.OrderBy(arg => districtOrderNumbers[arg.HighestDistrict])
				.ThenBy(arg => arg.Number)
				.Select((arg, index) => new
				{
					arg.DistrictName,
					arg.Value,
					arg.HighestDistrict,
					Index = index
				})
				.GroupBy(arg => arg.HighestDistrict)
				.Select(grouping => new ScatterplotChartSeries
				{
					Name = grouping.Key.Name,
					Tooltip = new SeriesTooltipOptions
					{
						PointFormat = "{point.name}<br />{point.y:.1f}%"
					},
					Data = grouping
						.Select(arg => new Point
						{
							Name = arg.DistrictName,
							X = arg.Index,
							Y = arg.Value
						})
						.ToArray()
				})
				.Cast<ChartSeries>()
				.ToArray();

			return new HighchartsOptions
			{
				YAxis = new AxisOptions
				{
					Min = 0,
					Max = 100,
					Title = new TitleOptions
					{
						Text = "% за кандидата"
					}
				},
				XAxis = new AxisOptions
				{
					Labels = new AxisLabels
					{
						Enabled = false
					}
				},
				Series = series
			};
		}

		public class ChartBuildParameters
		{
			public int ElectionId { get; set; }
			public int? DistrictId { get; set; }
			public int CandidateId { get; set; }

			public IQueryable<ElectionResult> GetElectionResults(ModelContext modelContext)
			{
				var results = modelContext.ElectionResults.ByElection(ElectionId);
				if (DistrictId != null)
				{
					var district = modelContext.ElectoralDistricts.GetById(DistrictId.Value);
					results = results.ByHigherDistrict(district);
				}
				return results;
			}

			public IQueryable<ElectoralDistrict> GetHighestDistricts(ModelContext modelContext)
			{
				if (DistrictId != null)
				{
					return modelContext.ElectoralDistricts.ByHigherDistrict(DistrictId.Value);
				}
				else
				{
					var election = modelContext.Elections.GetById(ElectionId);
					return modelContext.ElectoralDistricts.ByHigherDistrict(election.ElectoralDistrictId);
				}
			}
		}

		public class HistogramBuildParameters : ChartBuildParameters
		{
			public double StepSize { get; set; }
		}
	}
}