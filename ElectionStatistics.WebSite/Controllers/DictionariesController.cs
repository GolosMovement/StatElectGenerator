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

		[HttpGet, Route("candidates/by-election")]
		public IEnumerable<CandidateDto> GetCandidatesByElection(int electionId)
		{
			return modelContext.Candidates
				.ByElection(modelContext, electionId)
				.Select(election => new CandidateDto
				{
					Id = election.Id,
					Name = election.ShortName
				})
				.ToArray();
		}

		public class ElectionDto
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		public class CandidateDto
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
	}
}
