using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectionStatistics.Model;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

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

		[HttpGet, Route("histogram"), ResponseCache(CacheProfileName = "Default")]
		public HighchartsOptions GetDataForHistogram(string parametersString)
		{
			var parameters = DeserialzeJson<HistogramBuildParameters>(parametersString);

			var results = parameters.GetElectionResults(modelContext);

			var data = results
				.Join(
					parameters.X.GetParameters(modelContext),
					result => result.Id,
					votes => votes.ElectionResultId,
					(result, parameterValue) => new
					{
						VotersCount = result.InsideBallotsCount + result.OutsideBallotsCount,
						Value = parameterValue.Value
					})
				.ToArray() // На SQL сервере возникают ошибки округления
				.GroupBy(arg => Math.Round(arg.Value / parameters.StepSize) * parameters.StepSize)
				.Select(grouping => new Point
				{
					X = grouping.Key,
					Y = grouping.Sum(arg => arg.VotersCount)
				})
				.OrderBy(point => point.X)
				.ToArray();

			return new HighchartsOptions
			{
				XAxis = new AxisOptions
				{
					Min = 0,
					Max = 100
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
						Name = parameters.X.GetName(modelContext),
						Data = data
					}
				}
			};
		}

		[HttpGet, Route("scatterplot"), ResponseCache(CacheProfileName = "Default")]
		public HighchartsOptions GetDataForScatterplot(string parametersString)
		{
			var parameters = DeserialzeJson<ChartBuildParameters>(parametersString);

			var results = parameters.GetElectionResults(modelContext);

			ElectoralDistrict[] highestDistricts;
			if (parameters.DistrictId != null)
			{
				var highestDistrictsQueryable = modelContext.ElectoralDistricts.ByHigherDistrict(parameters.DistrictId.Value);
				highestDistricts = highestDistrictsQueryable.Any(district => !district.LowerDistricts.Any()) 
					? new [] { modelContext.ElectoralDistricts.GetById(parameters.DistrictId.Value) } 
					: highestDistrictsQueryable.ToArray();
			}
			else
			{
				var election = modelContext.Elections.GetById(parameters.ElectionId);
				highestDistricts = modelContext.ElectoralDistricts.ByHigherDistrict(election.ElectoralDistrictId).ToArray();
			}

			var sourceData = results
				.Join(
					parameters.Y.GetParameters(modelContext),
					result => result.Id,
					votes => votes.ElectionResultId,
					(result, parameterValue) => new
					{
						result.ElectoralDistrict.Id,
						DistrictName = result.ElectoralDistrict.Name,
						Value = parameterValue.Value,
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

			var seriesGrouping = sourceData
				.OrderBy(arg => districtOrderNumbers[arg.HighestDistrict])
				.ThenBy(arg => arg.HighestDistrict.Name)
				.ThenBy(arg => arg.Number)
				.Select((arg, index) => new
				{
					arg.DistrictName,
					arg.Value,
					arg.HighestDistrict,
					Index = index
				})
				.GroupBy(arg => arg.HighestDistrict);

			var highchartsOptions = new HighchartsOptions
			{
				YAxis = new AxisOptions
				{
					Min = 0,
					Max = 100,
					Title = new TitleOptions
					{
						Text = parameters.Y.GetName(modelContext)
					}
				},
				XAxis = new AxisOptions
				{
					Min = 0,
					Max = sourceData.Length,
					Labels = new AxisLabels
					{
						Enabled = false
					}
				}
			};
			if (sourceData.Length >= 10000)
			{
				highchartsOptions.Legend = new LegendOptions { Enabled = false };

				highchartsOptions.Series = seriesGrouping
					.Select(grouping => new FastScatterplotChartSeries
					{
						Name = grouping.Key.Name,
						Tooltip = new SeriesTooltipOptions
						{
							PointFormat = "{point.y:.1f}%"
						},
						Data = grouping
							.Select(arg => new[]
							{
								arg.Index,
								arg.Value
							})
							.ToArray()
					})
					.Cast<ChartSeries>()
					.ToArray();
			}
			else
			{
				highchartsOptions.Series = seriesGrouping
					.Select(grouping => new FullScatterplotChartSeries
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
			}
				

			return highchartsOptions;
		}

		private TValue DeserialzeJson<TValue>(string jsonString)
		{
			return JsonConvert.DeserializeObject<TValue>(jsonString, new JsonSerializerSettings
			{
				SerializationBinder = new AttributeSerializationBinder(),
				TypeNameHandling = TypeNameHandling.Auto
			});
		}

		public class ChartBuildParameters
		{
			public int ElectionId { get; set; }
			public int? DistrictId { get; set; }
			public ChartParameter X { get; set; }
			public ChartParameter Y { get; set; }

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
		}

		public class HistogramBuildParameters : ChartBuildParameters
		{
			public decimal StepSize { get; set; }
		}
	}
}
