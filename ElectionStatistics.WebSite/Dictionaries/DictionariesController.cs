using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectionStatistics.Model;
using Microsoft.AspNetCore.Mvc;

namespace ElectionStatistics.WebSite
{
	[Route("api")]
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
				.ByElection(modelContext, electionId)
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
			return modelContext.Candidates
				.ByElection(modelContext, electionId)
				.Select(candidate => new CandidateVotePercentageChartParameter
				{
					CandidateId = candidate.Id
				})
				.ToArray()
				.Concat(new ChartParameter[]
				{
					new AttendancePercentageChartParameter()
				})
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
	}
}
