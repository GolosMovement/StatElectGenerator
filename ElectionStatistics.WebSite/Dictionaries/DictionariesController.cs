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

		[HttpGet, Route("elections")]
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

		[HttpGet, Route("districts")]
		public IEnumerable<ElectoralDistrictDto> GetDistrict(int electionId, bool forScatterplot)
		{
			var election = modelContext.Elections.GetById(electionId);
			var districtsByHigherDistrtict = modelContext.ElectoralDistricts
				.ByElection(modelContext, electionId)
				.ToArray()
				.GroupBy(district => district.HigherDistrictId)
				.ToDictionary(
					grouping => grouping.Key.Value,
					grouping => grouping.ToArray());

			if (forScatterplot)
			{
				var lowestDistrictIds = districtsByHigherDistrtict
					.Where(pair => !districtsByHigherDistrtict.ContainsKey(pair.Value.First().Id))
					.Select(pair => pair.Key)
					.ToArray();
				foreach (var lowestDistrictId in lowestDistrictIds)
				{
					districtsByHigherDistrtict.Remove(lowestDistrictId);
				}
			}

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

		[HttpGet, Route("candidates")]
		public IEnumerable<CandidateDto> GetCandidates(int electionId)
		{
			return modelContext.Candidates
				.ByElection(modelContext, electionId)
				.Select(candidate => new CandidateDto
				{
					Id = candidate.Id,
					Name = candidate.ShortName
				})
				.ToArray();
		}

		public class CandidateDto
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
	}
}
