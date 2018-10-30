using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Methods
{
    public class LocationScatterplotBuildParameters : ChartBuildParameters
    {
        public int Y { get; set; }
        public decimal StepSize { get; set; }
        public IQueryable<LineCalculatedValue> GetLineCalculatedValues(ModelContext modelContext)
        {
            var baseQuery =
            @"%hierarchyQuery%
                SELECT
                    LineCalculatedValues.Id
                    ,LineCalculatedValues.ProtocolId
                    ,LineCalculatedValues.PresetId
                    ,LineCalculatedValues.Value
                FROM LineCalculatedValues
                WHERE
                    LineCalculatedValues.PresetId = @preset
                    %protocolFilter%";

            var hierarchyQuery =
                @"WITH query AS
                    (SELECT *
                    FROM Protocols p1
                    WHERE p1.id = @protocol
                    UNION ALL
                    SELECT p2.*
                    FROM Protocols p2
                    JOIN query ON p2.ParentId = query.Id)";

            var protocolFilter =
                @"AND LineCalculatedValues.ProtocolId in (SELECT Id FROM query)";

            var sql = baseQuery;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@preset", Y));

            if (ProtocolId != null)
            {
                sql = sql.Replace("%hierarchyQuery%", hierarchyQuery)
                    .Replace("%protocolFilter%", protocolFilter);

                sqlParameters.Add(new SqlParameter("@protocol", ProtocolId));
            }
            else
            {
                sql = sql.Replace("%hierarchyQuery%", "").Replace("%protocolFilter%", "");
            }

            var lineCalculatedValues = modelContext.Set<LineCalculatedValue>().AsNoTracking()
                .FromSql(sql, sqlParameters.Cast<object>().ToArray());
            return lineCalculatedValues;
        }
    }

    public class LocationScatterplot
    {
        public struct Grouping
        {
            public int Index { get; set; }
            public string ProtocolName { get; set; }
            public double Y { get; set; }
            public Protocol TopParentProtocol { get; set; }
        }

        private ModelContext modelContext;

        public LocationScatterplot(ModelContext modelContext)
        {
            this.modelContext = modelContext;
        }

        public IEnumerable<IGrouping<Protocol, Grouping>> GetData(
            LocationScatterplotBuildParameters parameters,
            LineCalculatedValue[] values,
            Protocol[] higherProtocols)
        {
            var protocols = modelContext.Set<Protocol>().AsNoTracking()
                .Where(p => p.ProtocolSetId == parameters.ProtocolSetId);

            var sourceData = values
                .Join(
                    protocols,
                    result => result.ProtocolId,
                    protocol => protocol.Id,
                    (result, protocol) => new
                    {
                        Number = ComissionNumber(protocol),
                        ProtocolName = protocol.TitleRus,
                        Y = result.Value,
                        TopParentProtocol = FindTopParentProtocol(protocol, higherProtocols)
                    })
                    .ToArray();

            var districtOrderNumbers = sourceData
                .GroupBy(arg => arg.TopParentProtocol)
                .ToDictionary(grouping => grouping.Key,
                    grouping => grouping.Min(arg => arg.Number));

            var results = sourceData
                .OrderBy(arg => districtOrderNumbers[arg.TopParentProtocol])
                .ThenBy(arg => arg.TopParentProtocol.TitleRus)
                .ThenBy(arg => arg.Number)
                .Select((arg, index) => new Grouping
                {
                    ProtocolName = arg.ProtocolName,
                    Y = arg.Y,
                    TopParentProtocol = arg.TopParentProtocol,
                    Index = index
                })
                .GroupBy(arg => arg.TopParentProtocol);

            return results;
        }

        private int ComissionNumber(Protocol protocol)
        {
            if (protocol.CommissionNumber > 0)
            {
                return protocol.CommissionNumber;
            }
            else
            {
                return int.Parse(protocol.TitleRus.Replace("УИК №", ""));
            }
        }

        private Protocol FindTopParentProtocol(Protocol protocol, Protocol[] allProtocols)
        {
            if (protocol.ParentId != null)
            {
                protocol.Parent = allProtocols.SingleOrDefault(p => p.Id == protocol.ParentId);
                if (protocol.Parent != null)
                {
                    return FindTopParentProtocol(protocol.Parent, allProtocols);
                }
                else
                {
                    return protocol;
                }
            }
            else
            {
                return protocol;
            }
        }
    }
}
