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
			var results = modelContext.ElectionResults.ByElection(parameters.ElectionId);
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
				.GroupBy(arg => Math.Ceiling(arg.Value / parameters.StepSize) * parameters.StepSize)
				.Select(grouping => new []
				{
					grouping.Key,
					grouping.Sum(arg => arg.VotersCount)
				})
				.ToArray();

			return new[]
			{
				new ChartSeriesDto
				{
					Name = candidate.ShortName,
					Data = data
				}
			};
		}

		[HttpGet, Route("scatterplot")]
		public IEnumerable<ChartSeriesDto> GetDataForScatterplot(ChartBuildParameters parameters)
		{
			var results = modelContext.ElectionResults.ByElection(parameters.ElectionId);
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
							: (double)votes.Count * 100 / (result.InsideBallotsCount + result.OutsideBallotsCount)
					})
				.Select(arg => new[]
				{
					arg.Value,
					arg.VotersCount
				})
				.ToArray();

			return new[]
			{
				new ChartSeriesDto
				{
					Name = candidate.ShortName,
					Data = data
				}
			};
		}

		public class ChartBuildParameters
		{
			public int ElectionId { get; set; }
			public int CandidateId { get; set; }
		}

		public class HistogramBuildParameters : ChartBuildParameters
		{
			public double StepSize { get; set; }
		}

		public class ChartSeriesDto
		{
			public string Name { get; set; }
			public double[][] Data { get; set; }
	}
	}
}
