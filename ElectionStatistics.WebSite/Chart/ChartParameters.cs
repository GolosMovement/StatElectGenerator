using System;
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
		public virtual decimal? MinValue => null;
		public virtual decimal? MaxValue => null;

		public abstract string GetName(ModelContext modelContext);

		public abstract IQueryable<ParameterValue> GetParameters(ModelContext modelContext);
		
		public class ParameterValue
		{
			public decimal Value { get; set; }
			public int ElectionResultId { get; set; }
		}
	}

	public abstract class PercentageChartParameter : ChartParameter
	{
		public sealed override decimal? MinValue => 0;
		public sealed override decimal? MaxValue => 100;

	}

	[DataContract(Name = "candidateVotePercentage")]
	public class CandidateVotePercentageChartParameter : PercentageChartParameter
	{
		[DataMember(Name = "candidateId")]
		public int CandidateId { get; set; }

		public override string GetName(ModelContext modelContext) => modelContext.Candidates.GetById(CandidateId).ShortName + " (%)";

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

	[DataContract(Name = "attendancePercentage")]
	public class AttendancePercentageChartParameter : PercentageChartParameter
	{
		public override string GetName(ModelContext modelContext) => "Явка (%)";

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

	[DataContract(Name = "outsideVotersPercentage")]
	public class OutsideVotersPercentageChartParameter : PercentageChartParameter
	{
		public override string GetName(ModelContext modelContext) => "Проголосовавшие вне участка (%)";

		public override IQueryable<ParameterValue> GetParameters(ModelContext modelContext)
		{
			return modelContext.ElectionResults
				.Select(result => new ParameterValue
				{
					ElectionResultId = result.Id,
					Value = result.InsideBallotsCount + result.OutsideBallotsCount == 0
						? 0
						: (decimal)result.OutsideBallotsCount * 100 / (result.InsideBallotsCount + result.OutsideBallotsCount)
				});
		}
	}

	[DataContract(Name = "absenteeCertificateVotersPercentage")]
	public class AbsenteeCertificateVotersPercentageChartParameter : PercentageChartParameter
	{
		public override string GetName(ModelContext modelContext) => "Проголосовавшие по открепительным (%)";

		public override IQueryable<ParameterValue> GetParameters(ModelContext modelContext)
		{
			return modelContext.ElectionResults
				.Select(result => new ParameterValue
				{
					ElectionResultId = result.Id,
					Value = result.InsideBallotsCount + result.OutsideBallotsCount == 0
						? 0
						: (decimal) result.AbsenteeCertificateVotersCount * 100 / (result.InsideBallotsCount + result.OutsideBallotsCount)
				});
		}
	}

	[DataContract(Name = "invalidBallotsPercentage")]
	public class InvalidBallotsPercentageChartParameter : PercentageChartParameter
	{
		public override string GetName(ModelContext modelContext) => "Недействительные бюллетени (%)";

		public override IQueryable<ParameterValue> GetParameters(ModelContext modelContext)
		{
			return modelContext.ElectionResults
				.Select(result => new ParameterValue
				{
					ElectionResultId = result.Id,
					Value = result.InsideBallotsCount + result.OutsideBallotsCount == 0
						? 0
						: (decimal)result.InvalidBallotsCount * 100 / (result.InsideBallotsCount + result.OutsideBallotsCount)
				});
		}
	}

	public abstract class VotersCountBaseChartParameter : ChartParameter
	{
		public override IQueryable<ParameterValue> GetParameters(ModelContext modelContext)
		{
			return modelContext.ElectionResults
				.Select(result => new ParameterValue
				{
					ElectionResultId = result.Id,
					Value = result.VotersCount
				});
		}
	}

	[DataContract(Name = "votersCount")]
	public class VotersCountChartParameter : VotersCountBaseChartParameter
	{
		public override string GetName(ModelContext modelContext) => "Количество избирателей, зарегистрированных на участке";
	}


	[DataContract(Name = "summaryVotersCount")]
	public class SummaryVotersCountChartParameter : VotersCountBaseChartParameter
	{
		public override string GetName(ModelContext modelContext) => "Суммарное количество избирателей, зарегистрированных на участках";
	}


	[DataContract(Name = "pollingStationsCount")]
	public class PollingStationsCountChartParameter : ChartParameter
	{
		public override string GetName(ModelContext modelContext) => "Количество участков";

		public override IQueryable<ParameterValue> GetParameters(ModelContext modelContext)
		{
			return modelContext.ElectionResults
				.Select(result => new ParameterValue
				{
					ElectionResultId = result.Id,
					Value = 1
				});
		}
	}
}