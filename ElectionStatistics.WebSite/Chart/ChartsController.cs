using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

// DEBUG
using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ElectionStatistics.Model;
using ElectionStatistics.Core.Methods;

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

            var data = parameters.X.GetParameters(modelContext)
                .Join(
                    results,
                    x => x.ElectionResultId,
                    result => result.Id,
                    (x, result) => x)
                .Join(
                    parameters.Y.GetParameters(modelContext),
                    x => x.ElectionResultId,
                    y => y.ElectionResultId,
                    (x, y) => new
                    {
                        X = x.Value,
                        Y = y.Value
                    })
                .ToArray() // На SQL сервере возникают ошибки округления
                .GroupBy(arg => Math.Round(arg.X / parameters.StepSize) * parameters.StepSize)
                .Select(grouping => new Point
                {
                    X = grouping.Key,
                    Y = grouping.Sum(arg => arg.Y)
                })
                .OrderBy(point => point.X)
                .ToArray();

            return new HighchartsOptions
            {
                XAxis = new AxisOptions
                {
                    Min = parameters.X.MinValue,
                    Max = parameters.X.MaxValue
                },
                YAxis = new AxisOptions
                {
                    Min = parameters.Y.MinValue,
                    Max = parameters.Y.MaxValue,
                    Title = new TitleOptions
                    {
                        Text = parameters.Y.GetName(modelContext)
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
            var parameters = DeserialzeJson<ScatterplotBuildParameters>(parametersString);

            var results = parameters.GetElectionResults(modelContext);

            var data = parameters.X.GetParameters(modelContext)
                .Join(
                    parameters.Y.GetParameters(modelContext),
                    x => x.ElectionResultId,
                    y => y.ElectionResultId,
                    (x, y) => new
                    {
                        x.ElectionResultId,
                        X = x.Value,
                        Y = y.Value
                    })
                .Join(
                    results,
                    x => x.ElectionResultId,
                    result => result.Id,
                    (arg, result) => new Point
                    {
                        Name = result.ElectoralDistrict.HigherDistrict.Name + " - " + result.ElectoralDistrict.Name,
                        X = arg.X,
                        Y = arg.Y
                    })
                .ToArray();

            var xName = parameters.X.GetName(modelContext);
            var yName = parameters.Y.GetName(modelContext);

            var highchartsOptions = new HighchartsOptions
            {
                XAxis = new AxisOptions
                {
                    Min = parameters.X.MinValue,
                    Max = parameters.X.MaxValue
                },
                YAxis = new AxisOptions
                {
                    Min = parameters.Y.MinValue,
                    Max = parameters.Y.MaxValue,
                    Title = new TitleOptions
                    {
                        Text = yName
                    }
                }
            };

            var series = new FullScatterplotChartSeries
            {
                Name = xName,
                Tooltip = new SeriesTooltipOptions
                {
                    PointFormat = $"{{point.name}}<br />{xName}: {{point.x:.1f}}<br/>{yName}: {{point.y:.1f}}"
                },
                Data = data
            };


            if (data.Length >= 10000)
            {
                highchartsOptions.Legend = new LegendOptions { Enabled = false };

                highchartsOptions.Series = new []
                {
                    series.ConvertToFastSeries($"{xName}: {{point.x:.1f}}<br/>{yName}: {{point.y:.1f}}")
                };
            }
            else
            {
                highchartsOptions.Series = new[]
                {
                    series
                };
            }
            return highchartsOptions;
        }

        class ProtocolDto : Protocol
        {
            public int N { get; set; }
        }

        [HttpGet, Route("location-scatterplot"), ResponseCache(CacheProfileName = "Default")]
        public HighchartsOptions GetDataForLocationScatterplot(string parametersString)
        {
            var sw = new Stopwatch();
            sw.Start();

            var parameters = DeserialzeJson<LocationScatterplotBuildParameters>(parametersString);

            var connection = (SqlConnection) modelContext.Database.GetDbConnection();
            CalculateValuesRepository cvr = new CalculateValuesRepository(connection);
            LocationScatterplotRepository lsr = new LocationScatterplotRepository(connection, cvr);

            var preset = modelContext.Find<Preset>(parameters.Y);

            Console.WriteLine(parameters.Y);
            Console.WriteLine(preset);

            var results = lsr.Query(parameters.ProtocolSetId, parameters.ProtocolId,
                preset.Expression);

            Console.WriteLine($"results = {results.Protocols.Count}");

            var highchartsOptions = new HighchartsOptions
            {
                XAxis = new AxisOptions
                {
                    Min = 0,
                    Max = results.Protocols.Count,
                    Labels = new AxisLabels
                    {
                        Enabled = false
                    }
                },
                YAxis = new AxisOptions
                {
                    Min = 0,
                    Max = results.MaxY,
                    Title = new TitleOptions
                    {
                        Text = preset.TitleRus
                    }
                }
            };

            var byParents = results.Protocols.GroupBy(p => p.ParentId);
            Console.WriteLine($"GROUPS:{byParents.Count()}");

            var series = new [] { new FullScatterplotChartSeries
            {
                Name = "",
                Tooltip = new SeriesTooltipOptions
                {
                    PointFormat = "{point.name}<br />{point.y:.1f}%"
                },

                Data = results.Protocols
                    .OrderBy(p => p.CommissionNumber)
                    .ThenBy(p => p.TitleRus)
                    .Select((p, index) => new Point
                        {
                            Name = p.TitleRus,
                            X = index,
                            Y = p.Y
                        })
                    .ToArray()
            }};

            highchartsOptions.Series = series;
            highchartsOptions.Legend = new LegendOptions { Enabled = false };

            sw.Stop();

            Console.WriteLine($"Time elapsed: {sw.Elapsed}");

            return highchartsOptions;
        }

        [HttpGet, Route("last-digit-analyzer"), ResponseCache(CacheProfileName = "Default")]
        public LastDigitAnalyzerData GetDataForLastDigitAnalyzer(string parametersString)
        {
            var parameters = DeserialzeJson<LastDigitAnalyzerBuildParameters>(parametersString);

            var ldaRepo = new LDARepository((SqlConnection) modelContext.Database.GetDbConnection());
            var ldaNumbers = ldaRepo.CountNumbers(
                parameters.ProtocolSetId,
                parameters.LineDescriptionIds,
                parameters.ProtocolId,
                parameters.MinValue);

            if (ldaNumbers == null)
            {
                return new LastDigitAnalyzerData();
            }

            var lda = new LastDigitAnalyzer();
            var ldaResult = lda.Calculate(ldaNumbers.Total, ldaNumbers.Numbers);

            if (ldaResult == null)
            {
                return new LastDigitAnalyzerData();
            }

            var highchartsOptions = new HighchartsOptions
            {
                YAxis = new AxisOptions
                {
                    Title = new TitleOptions { Text = "Частотность появления цифры" }
                }
            };

            var series = new LDAChartSeries
            {
                Data = ldaResult.Frequency.ToArray(), Name = "Частота", Type = "column"
            };

            highchartsOptions.Series = new ChartSeries[]
            {
                series,
                new LDAChartSeries
                {
                    Color = "blue", Name = "ожидаемое", Type = "line",
                    Data = Enumerable.Repeat(0.1, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "green", Name = "+1 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 + ldaResult.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "green", Name = "-1 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 - ldaResult.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "yellow", Name = "+2 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 + 2 * ldaResult.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "yellow", Name = "-2 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 - 2 * ldaResult.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "red", Name = "+3 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 + 3 * ldaResult.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "red", Name = "-3 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 - 3 * ldaResult.Sigma, 10).ToArray()
                }
            };

            return new LastDigitAnalyzerData {
                ChartOptions = highchartsOptions, ChiSquared = ldaResult.ChiSquared
            };
        }

        private TValue DeserialzeJson<TValue>(string jsonString)
        {
            return JsonConvert.DeserializeObject<TValue>(jsonString, new JsonSerializerSettings
            {
                SerializationBinder = new AttributeSerializationBinder(),
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        public class OldChartBuildParameters
        {
            public int ElectionId { get; set; }
            public int? DistrictId { get; set; }

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

        public class HistogramBuildParameters : OldChartBuildParameters
        {
            public ChartParameter X { get; set; }
            public ChartParameter Y { get; set; }
            public decimal StepSize { get; set; }
        }

        public class ScatterplotBuildParameters : OldChartBuildParameters
        {
            public ChartParameter X { get; set; }
            public ChartParameter Y { get; set; }
        }

        public class LastDigitAnalyzerBuildParameters
        {
            public int ProtocolSetId { get; set; }
            public int? ProtocolId { get; set; }
            public int[] LineDescriptionIds { get; set; }
            public int? MinValue { get; set; }
        }

        public class LastDigitAnalyzerData
        {
            public HighchartsOptions ChartOptions { get; set; }
            public double? ChiSquared { get; set; }
        }
    }
}
