using System.Linq;
using System.Runtime.Serialization;

using ElectionStatistics.Model;

namespace ElectionStatistics.WebSite
{
	[DataContract]
	public class NamedChartParameter
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "parameter")]
		public ChartParameter Parameter { get; set; }
	}

	[DataContract]
	public abstract class ChartParameter
	{
		public abstract string GetName(ModelContext modelContext);

		public abstract IQueryable<ParameterValue> GetParameters(ModelContext modelContext);
		
		public class ParameterValue
		{
			public decimal Value { get; set; }
			public int ElectionResultId { get; set; }
		}
	}

	[DataContract(Name = "candidate")]
	public class CandidateChartParameter : ChartParameter
	{
		[DataMember(Name = "candidateId")]
		public int CandidateId { get; set; }

		public override string GetName(ModelContext modelContext)
		{
			return modelContext.Candidates.GetById(CandidateId).ShortName + " (%)";
		}

		public override IQueryable<ParameterValue> GetParameters(ModelContext modelContext)
		{
			return modelContext.ElectionCandidatesVotes
				.ByCandidate(CandidateId)
				.Select(vote => new ParameterValue
				{
					ElectionResultId = vote.ElectionResultId,
					Value = vote.ElectionResult.InsideBallotsCount + vote.ElectionResult.OutsideBallotsCount == 0
						? 0
						: (decimal)vote.Count * 100 / (vote.ElectionResult.InsideBallotsCount + vote.ElectionResult.OutsideBallotsCount)
				});
		}
	}

	[DataContract(Name = "attendance")]
	public class AttendanceChartParameter : ChartParameter
	{
		public override string GetName(ModelContext modelContext)
		{
			return "Явка (%)";
		}

		public override IQueryable<ParameterValue> GetParameters(ModelContext modelContext)
		{
			return modelContext.ElectionResults
				.Select(result => new ParameterValue
				{
					ElectionResultId = result.Id,
					Value = result.InsideBallotsCount + result.OutsideBallotsCount == 0
						? 0
						: (decimal)(result.InsideBallotsCount + result.OutsideBallotsCount) * 100 / result.VotersCount
				});
		}
	}
}