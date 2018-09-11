using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ElectionStatistics.Model;

namespace ElectionStatistics.WebSite
{
    [Route("api")]
    [AllowAnyOriginCorsRequest]
    public class DictionariesController : Controller
    {
        private readonly ModelContext modelContext;

        public DictionariesController(ModelContext modelContext)
        {
            this.modelContext = modelContext;
        }

        [HttpGet, Route("elections"), ResponseCache(Duration = 300)]
        public IEnumerable<ElectionDto> GetElections()
        {
            return modelContext.Elections
                .Select(election => new ElectionDto
                {
                    Id = election.Id,
                    Name = election.Name
                })
                .ToArray();
        }

        public class ElectionDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [HttpGet, Route("districts"), ResponseCache(CacheProfileName = "Default")]
        public IEnumerable<ElectoralDistrictDto> GetDistrict(int electionId)
        {
            var election = modelContext.Elections.GetById(electionId);
            var districtsByHigherDistrtict = modelContext.ElectoralDistricts
                .ByElection(modelContext, electionId).AsNoTracking()
                .ToArray()
                .GroupBy(district => district.HigherDistrictId)
                .ToDictionary(
                    grouping => grouping.Key.Value,
                    grouping => grouping.ToArray());
            return districtsByHigherDistrtict.Values
                .SelectMany(districts => districts)
                .Where(district => district.HigherDistrictId == election.ElectoralDistrictId)
                .Select(district => BuildDistrictDto(district, districtsByHigherDistrtict))
                .OrderBy(dto => dto.Name)
                .ToArray();
        }

        private ElectoralDistrictDto BuildDistrictDto(
            ElectoralDistrict district,
            Dictionary<int, ElectoralDistrict[]> districtsByHigherDistrtict)
        {
            var result = new ElectoralDistrictDto
            {
                Id = district.Id,
                Name = district.Name
            };

            if (districtsByHigherDistrtict.TryGetValue(district.Id, out var lowerDistricts))
            {
                result.LowerDitsrticts = lowerDistricts
                    .Select(lowerDistrict => BuildDistrictDto(lowerDistrict, districtsByHigherDistrtict))
                    .OrderBy(dto => dto.Name)
                    .ToArray();
            }

            return result;
        }

        public class ElectoralDistrictDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ElectoralDistrictDto[] LowerDitsrticts { get; set; }
        }

        [HttpGet, Route("parameters"), ResponseCache(CacheProfileName = "Default")]
        public IEnumerable<NamedChartParameter> GetParameters(int electionId)
        {
            return new ChartParameter[]
                {
                    new AttendancePercentageChartParameter(),
                    new OutsideVotersPercentageChartParameter(),
                    new AbsenteeCertificateVotersPercentageChartParameter(),
                    new InvalidBallotsPercentageChartParameter()
                }
                .Concat(modelContext.Candidates
                    .ByElection(modelContext, electionId)
                    .Select(candidate => new CandidateVotePercentageChartParameter
                    {
                        CandidateId = candidate.Id
                    }))
                .Select(parameter => new NamedChartParameter
                {
                    Name = parameter.GetName(modelContext),
                    Parameter = parameter
                })
                .ToArray();
        }

        [HttpGet, Route("summary-parameters"), ResponseCache(CacheProfileName = "Default")]
        public IEnumerable<NamedChartParameter> GetSummaryParameters()
        {
            return new ChartParameter[]
                {
                    new SummaryVotersCountChartParameter(),
                    new PollingStationsCountChartParameter()
                }
                .Select(parameter => new NamedChartParameter
                {
                    Name = parameter.GetName(modelContext),
                    Parameter = parameter
                })
                .ToArray();
        }

        [HttpGet, Route("protocolSets")]
        public IEnumerable<ProtocolSet> ProtocolSets()
        {
            return modelContext.Set<ProtocolSet>().AsNoTracking()
                .Where(protocolSet => !protocolSet.Hidden)
                .OrderBy(protocolSet => protocolSet.TitleRus);
        }

        public struct ProtocolDto
        {
            public int Id { get; set; }
            public string TitleRus { get; set; }
            public string TitleEng { get; set; }
            public string TitleNative { get; set; }
            public ProtocolDto[] LowerProtocols { get; set; }
        }

        [HttpGet, Route("protocols")]
        public IEnumerable<ProtocolDto> GetProtocols(int protocolSetId)
        {
            var protocols = modelContext.Set<Protocol>().AsNoTracking()
                .Where(protocol => protocol.ProtocolSetId == protocolSetId &&
                    protocol.ParentId != null &&
                    modelContext.Set<Protocol>().Any(pr => pr.ParentId == protocol.Id)).ToArray()
                .GroupBy(protocol => protocol.ParentId)
                .ToDictionary(grouping => grouping.Key.Value, grouping => grouping.ToArray());
            var topProtocols = modelContext.Set<Protocol>()
                .Where(pt => pt.ParentId == null && pt.ProtocolSetId == protocolSetId);

            return topProtocols.OrderBy(protocol => protocol.TitleRus)
                .Select(protocol => BuildProtocolDto(protocol, protocols));
        }

        private ProtocolDto BuildProtocolDto(Protocol protocol,
            Dictionary<int, Protocol[]> protocols)
        {
            var result = new ProtocolDto
            {
                Id = protocol.Id,
                TitleRus = protocol.TitleRus,
                TitleEng = protocol.TitleEng,
                TitleNative = protocol.TitleNative
            };

            if (protocols.TryGetValue(protocol.Id, out var lowerProtocols))
            {
                result.LowerProtocols = lowerProtocols
                    .Select(lowerProtocol => BuildProtocolDto(lowerProtocol, protocols))
                    .OrderBy(dto => dto.TitleRus)
                    .ToArray();
            }

            return result;
        }

        public struct LineDescriptionDto
        {
            public int Id { get; set; }
            public string TitleRus { get; set; }
            public string TitleEng { get; set; }
            public string TitleNative { get; set; }
        }

        [HttpGet, Route("lineDescriptions")]
        public IEnumerable<LineDescriptionDto> GetLineDescriptions(int protocolSetId)
        {
            return modelContext.Set<LineDescription>().AsNoTracking()
                .Where(line => line.ProtocolSetId == protocolSetId && line.IsCalcResult)
                .OrderBy(line => line.DescriptionRus)
                .Select(line => new LineDescriptionDto()
                {
                    Id = line.Id,
                    TitleRus = line.TitleRus,
                    TitleEng = line.TitleEng,
                    TitleNative = line.TitleNative
                }).ToArray();
        }
    }
}
