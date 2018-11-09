using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
            var parameters = DeserialzeJson<LocationScatterplotBuildParameters>(parametersString);

            var results = parameters.GetLineCalculatedValues(modelContext).ToArray();

            Protocol[] higherProtocols;
            if (parameters.ProtocolId != null)
            {
                var sql = @"WITH query AS
                    (SELECT *
                       FROM protocols p1
                      WHERE p1.parentid = @p0
                      UNION ALL
                     SELECT p2.*
                       FROM protocols p2
                       JOIN query ON p2.ParentId = query.Id)
                    SELECT * FROM query
                     WHERE EXISTS (SELECT 1 FROM protocols WHERE protocols.parentid = query.id)";
                higherProtocols = modelContext.Set<Protocol>().FromSql(sql, parameters.ProtocolId)
                    .DefaultIfEmpty(modelContext.Find<Protocol>(parameters.ProtocolId)).ToArray();
            }
            else
            {
                higherProtocols = modelContext.Set<Protocol>().AsNoTracking()
                    .Where(p => p.ProtocolSetId == parameters.ProtocolSetId &&
                        modelContext.Set<Protocol>().Any(pp => pp.Id == p.ParentId)).ToArray();
            }
            var data = new LocationScatterplot(modelContext)
                .GetData(parameters, results, higherProtocols);

            var highchartsOptions = new HighchartsOptions
            {
                XAxis = new AxisOptions
                {
                    Min = 0,
                    Max = results.Length,
                    Labels = new AxisLabels
                    {
                        Enabled = false
                    }
                },
                YAxis = new AxisOptions
                {
                    Min = 0,
                    Max = 100,
                    Title = new TitleOptions
                    {
                        Text = modelContext.Find<Preset>(parameters.Y).TitleRus
                    }
                }
            };

            var series = data
                .Select(grouping => new FullScatterplotChartSeries
                {
                    Name = grouping.Key.TitleRus,
                    Tooltip = new SeriesTooltipOptions
                    {
                        PointFormat = "{point.name}<br />{point.y:.1f}%"
                    },
                    Data = grouping
                        .Select(
                            arg => new Point
                            {
                                Name = arg.ProtocolName,
                                X = arg.Index,
                                Y = Convert.ToDecimal(arg.Y) * 100
                            })
                        .ToArray()
                });

            if (results.Length >= 10000)
            {
                highchartsOptions.Legend = new LegendOptions { Enabled = false };

                highchartsOptions.Series = series
                    .Select(s => s.ConvertToFastSeries("{point.y:.1f}%"))
                    .ToArray();
            }
            else
            {
                highchartsOptions.Series = series.ToArray();
            }

            return highchartsOptions;
        }

        [HttpGet, Route("last-digit-analyzer"), ResponseCache(CacheProfileName = "Default")]
        public LastDigitAnalyzerData GetDataForLastDigitAnalyzer(string parametersString)
        {
            var parameters = DeserialzeJson<LastDigitAnalyzerBuildParameters>(parametersString);

            var baseQuery =
                @"%hierarchyQuery%
                SELECT
                    LineNumbers.Id
                    ,LineNumbers.ProtocolId
                    ,LineNumbers.LineDescriptionId
                    ,LineNumbers.Value
                FROM LineNumbers
                JOIN LineDescriptions ON (LineDescriptions.Id = LineNumbers.LineDescriptionId)
                WHERE
                    LineDescriptions.ProtocolSetId = @protocol_set
                    AND LineNumbers.Value IS NOT NULL
                    %lineDescFilter%
                    %protocolFilter%
                    %minValueFilter%";

            var hierarchyQuery =
                @"WITH query AS
                    (SELECT *
                    FROM Protocols p1
                    WHERE p1.id = @protocol
                    UNION ALL
                    SELECT p2.*
                    FROM Protocols p2
                    JOIN query ON p2.ParentId = query.Id)";

            var lineDescFilter =
                @"AND LineDescriptions.Id IN (%lineDescriptions%)";

            var protocolFilter = @"AND LineNumbers.ProtocolId IN (SELECT Id FROM query)";

            var minValueFilter = @"AND LineNumbers.Value >= @min_value";

            var sql = baseQuery;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@protocol_set", parameters.ProtocolSetId));

            if (parameters.LineDescriptionIds.Count() > 0)
            {
                sql = sql.Replace("%lineDescFilter%", lineDescFilter)
                    // FIXME: possible SQL injection
                    .Replace("%lineDescriptions%", string.Join(",", parameters.LineDescriptionIds));
            }
            else
            {
                sql = sql.Replace("%lineDescFilter%", "");
            }

            if (parameters.ProtocolId != null)
            {
                sql = sql.Replace("%hierarchyQuery%", hierarchyQuery)
                    .Replace("%protocolFilter%", protocolFilter);

                sqlParameters.Add(new SqlParameter("@protocol", parameters.ProtocolId));
            }
            else
            {
                sql = sql.Replace("%hierarchyQuery%", "").Replace("%protocolFilter%", "");
            }

            if (parameters.MinValue != null)
            {
                sql = sql.Replace("%minValueFilter%", minValueFilter);

                sqlParameters.Add(new SqlParameter("@min_value", parameters.MinValue));
            }
            else
            {
                sql = sql.Replace("%minValueFilter%", "");
            }

            // TODO: query plain values without an entity
            var lns = modelContext.Set<LineNumber>().FromSql(sql,
                    sqlParameters.Cast<object>().ToArray())
                .Select(l => l.Value).ToList();

            LDAResult results;
            try
            {
                results = new LastDigitAnalyzer().GetData(lns);
            }
            catch (ArgumentException)
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
                Data = results.Frequency.ToArray(), Name = "Частота", Type = "column"
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
                    Data = Enumerable.Repeat(0.1 + results.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "green", Name = "-1 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 - results.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "yellow", Name = "+2 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 + 2 * results.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "yellow", Name = "-2 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 - 2 * results.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "red", Name = "+3 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 + 3 * results.Sigma, 10).ToArray()
                },
                new LDAChartSeries
                {
                    Color = "red", Name = "-3 сигма", Type = "line",
                    Data = Enumerable.Repeat(0.1 - 3 * results.Sigma, 10).ToArray()
                }
            };

            return new LastDigitAnalyzerData {
                ChartOptions = highchartsOptions, ChiSquared = results.ChiSquared
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
