using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectionStatistics.Model;
using Microsoft.AspNetCore.Mvc;

namespace ElectionStatistics.WebSite
{
	[Route("api/elections")]
	public class ElectionsController : Controller
	{
		private readonly ModelContext modelContext;

		public ElectionsController(ModelContext modelContext)
		{
			this.modelContext = modelContext;
		}

		[HttpGet]
		public IEnumerable<ElectionDto> GetAll()
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
	}
}
