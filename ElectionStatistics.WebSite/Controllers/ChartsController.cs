using System;
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
		public IEnumerable<ChartSeriesDto> GetDataForHistogram(HistogramBuildParameters parameters)
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

			return new[]
			{
				new SimpleArrayChartSeriesDto
				{
					Name = candidate.ShortName,
					Data = data
				}
			};
		}

		[HttpGet, Route("scatterplot")]
		public IEnumerable<ChartSeriesDto> GetDataForScatterplot(ChartBuildParameters parameters)
		{
			var results = parameters.GetElectionResults(modelContext);
			var candidate = modelContext.Candidates.GetById(parameters.CandidateId);
			var highestDistricts = parameters.GetHighestDistricts(modelContext).ToArray();
			return results
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
					arg.Id,
					arg.DistrictName,
					arg.Value,
					HighestDistrict = highestDistricts.Single(district => 
						arg.HierarchyPath.StartsWith(district.GetChildrenHierarchyPath()))
				})
				.OrderBy(arg => arg.HighestDistrict.Name)
				.ThenBy(arg => arg.Id)
				.Select((arg, index) => new
				{
					arg.Id,
					arg.DistrictName,
					arg.Value,
					arg.HighestDistrict,
					Index = index
				})
				.GroupBy(arg => arg.HighestDistrict)
				.Select(grouping => new PointsChartSeriesDto
				{
					Name = grouping.Key.Name,
					Data = grouping
						.Select(arg => new PointDto
						{
							Name = arg.DistrictName,
							X = arg.Index,
							Y = arg.Value
						})
						.ToArray()
				})
				.ToArray();
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

		public class ChartSeriesDto
		{
			public string Name { get; set; }
		}

		public class SimpleArrayChartSeriesDto : ChartSeriesDto
		{
			public double[][] Data { get; set; }
		}

		public class PointsChartSeriesDto : ChartSeriesDto
		{
			public PointDto[] Data { get; set; }
		}

		public class PointDto
		{
			public string Name { get; set; }
			public double X { get; set; }
			public double Y { get; set; }
		}
	}
}
