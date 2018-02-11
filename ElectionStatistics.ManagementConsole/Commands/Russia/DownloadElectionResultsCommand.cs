using ElectionStatistics.Model;
using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace ElectionStatistics.ManagementConsole
{
	internal class DownloadElectionResultsCommand : Command
	{
		private RestClient client = new RestClient();

		public override string Name => "Download-ElectionResults";

		public override void Execute(IServiceProvider services, string[] arguments)
		{
			if (arguments.Length != 1)
				throw new ArgumentException("arguments.Length != 1");

			var vrn = arguments[0];

			var districts =
				GetInnerDistricts(null, vrn, 3);

			var candidateNames = new HashSet<string>();

			foreach (var lowestDistrict in districts.SelectMany(i => GetLowestDistricts(i)))
			{
				var districtResults = GetElectionResults(lowestDistrict.Vibid);

				districtResults
					.ForEach(
						r => candidateNames.UnionWith(r.Votes.Keys));

				lowestDistrict.InnerDistricts = districtResults
				.Select(r => new ElectoralDistrictDto
					{
						Name = r.ElectoralDistrictName,
						Results = r
					})
				.ToList();
			}

			using (var modelContext = services.GetService<ModelContext>())
			{
				var rootDistrict = modelContext.ElectoralDistricts.GetOrAdd("Россия", null);

				var election = modelContext.Elections.GetOrAdd("Выборы в госдуму 2016", rootDistrict, new DateTime(2016, 09, 18), vrn);

				var candidates = UploadCandidates(modelContext, election, candidateNames);
				modelContext.SaveChanges();

				UploadDistricts(modelContext, districts, rootDistrict, election, candidates);
				modelContext.SaveChanges();
			}
		}

		private IEnumerable<ElectoralDistrictDto> GetLowestDistricts(ElectoralDistrictDto district)
		{
			if (district.InnerDistricts == null)
				return Enumerable.Repeat(district, 1);

			return district.InnerDistricts.SelectMany(i => GetLowestDistricts(i));
		}

		private Dictionary<string, Candidate> UploadCandidates(
			ModelContext modelContext, 
			Election election,
			IEnumerable<string> candidateNames)
		{
			var result = new Dictionary<string, Candidate>();

			foreach (var candidateName in candidateNames)
			{
				var candidate = modelContext.Candidates.GetOrAdd(candidateName, election);

				result[candidateName] = candidate;
			}

			return result;
		}

		private void UploadDistricts(
			ModelContext modelContext,
			List<ElectoralDistrictDto> dtos, 
			ElectoralDistrict higherDistrict,
			Election election,
			Dictionary<string, Candidate> candidates)
		{
			dtos.ForEach(dto =>
			{
				var district = new ElectoralDistrict(dto.Name, higherDistrict);

				modelContext.ElectoralDistricts.Import(district);

				if (dto.InnerDistricts != null)
					UploadDistricts(
						modelContext, 
						dto.InnerDistricts, 
						district, 
						election, 
						candidates);

				if (dto.Results != null)
					UploadResults(
						modelContext,
						dto.Results,
						district,
						election,
						candidates);
			});
		}

		private void UploadResults(
			ModelContext modelContext,
			ElectionResultsDto dto,
			ElectoralDistrict district,
			Election election,
			Dictionary<string, Candidate> candidates)
		{
			var result = new ElectionResult()
			{
				ElectoralDistrict = district,
				Election = election,
				DataSourceUrl = "-",
				VotersCount = dto.VotersCount,
				ReceivedBallotsCount = dto.ReceivedBallotsCount,
				EarlyIssuedBallotsCount = dto.EarlyIssuedBallotsCount,
				IssuedInsideBallotsCount = dto.IssuedInsideBallotsCount,
				IssuedOutsideBallotsCount = dto.IssuedOutsideBallotsCount,
				CanceledBallotsCount = dto.CanceledBallotsCount,
				OutsideBallotsCount = dto.OutsideBallotsCount,
				InsideBallotsCount = dto.InsideBallotsCount,
				InvalidBallotsCount = dto.InvalidBallotsCount,
				ValidBallotsCount = dto.ValidBallotsCount,
				ReceivedAbsenteeCertificatesCount = dto.ReceivedAbsenteeCertificatesCount,
				IssuedAbsenteeCertificatesCount = dto.IssuedAbsenteeCertificatesCount,
				AbsenteeCertificateVotersCount = dto.AbsenteeCertificateVotersCount,
				CanceledAbsenteeCertificatesCount = dto.CanceledAbsenteeCertificatesCount,
				IssuedByHigherDistrictAbsenteeCertificatesCount = dto.IssuedByHigherDistrictAbsenteeCertificatesCount,
				LostAbsenteeCertificatesCount = dto.LostAbsenteeCertificatesCount,
				LostBallotsCount = dto.LostBallotsCount,
				UnaccountedBallotsCount = dto.UnaccountedBallotsCount
			};

			modelContext.ElectionResults.Add(result);

			dto
				.Votes
				.ToList()
				.ForEach(voteDto =>
				{
					var vote = new ElectionCandidateVote
					{
						ElectionResult = result,
						Candidate = candidates[voteDto.Key],
						Count = voteDto.Value
					};

					modelContext.ElectionCandidatesVotes.Add(vote);
				});
		}

		private string Get(string uri)
		{
			client.BaseUrl = new Uri(uri);
			var request = new RestRequest(Method.GET);
			var response = client.Execute(request);
			Encoding encoding = Encoding.GetEncoding(1251);
			return encoding.GetString(response.RawBytes);
		}

		private List<ElectoralDistrictDto> GetInnerDistricts(string vibid, string vrn, int maxNestingLevel)
		{
			if (maxNestingLevel <= 0)
				return null;

			string url;

			if (vibid == null)
				url = $"http://www.vybory.izbirkom.ru/region/izbirkom?action=show&global=1&vrn={vrn}&region=0&prver=0&pronetvd=0";
			else
				url = $"http://www.vybory.izbirkom.ru/region/izbirkom?action=show&global=1&vrn={vrn}&region=0&prver=0&pronetvd=0&vibid={vibid}";

			var selector = new Regex(@"<option\s+value=""[^""]+vibid=([0-9]+)[^""]*"">\s*([^<]+)<\/option>");

			var result = Get(url);

			return selector
				.Matches(result)
				.OfType<Match>()
				.Select(m => new ElectoralDistrictDto
				{
					Name = m.Groups[2].Value,
					Vibid = m.Groups[1].Value,
					InnerDistricts = GetInnerDistricts(m.Groups[1].Value, vrn, maxNestingLevel - 1)
				})
				.ToList();
		}

		public List<ElectionResultsDto> GetElectionResults(string vibid)
		{
			var response = Get($"http://www.vybory.izbirkom.ru/region/region/izbirkom?action=show&global=true&region=1&sub_region=1&vibid={vibid}&type=233");
			var doc = new HtmlDocument();

			doc.LoadHtml(response);

			var candidatesTable = doc
				.DocumentNode
				.SelectNodes("//*[contains(., 'Число избирателей')]")
				.Last()
				.ParentNode
				.ParentNode
				.ParentNode;

			var resultsTable = candidatesTable
				.ParentNode
				.ParentNode
				.Elements("td")
				.Last()
				.Element("div")
				.Element("table");

			var metrics = candidatesTable
				.Elements("tr")
				.Select(e => e.Elements("td").ToList())
				.Select(tds => new {
					Metric = tds[1].Element("nobr").InnerText.Trim(' '),
					Value = tds.Last().Element("nobr").Element("b").InnerText.Trim(' ')
				})
				.ToList();

			var results = resultsTable
				.Elements("tr")
				.Select(e => e
					.Elements("td")
					.Select(td => td.Element("nobr"))
					.Select(td => td?.Element("b")?.InnerText ?? td?.InnerText)
					.ToList())
				.ToList();

			var result = new List<ElectionResultsDto>();

			for (int i = 0; i < metrics.Count; i++)
			{
				if (i == 0)
				{
					result = results[i].Select(r => new ElectionResultsDto { ElectoralDistrictName = r }).ToList();
					continue;
				}

				if (metrics[i].Metric == "&nbsp;")
					continue;

				if (metrics[i].Metric.StartsWith("Число"))
				{
					for (int j = 0; j < results[i].Count(); j++)
						electionResultsFieldParsers[metrics[i].Metric](result[j], int.Parse(results[i][j]));

					continue;
				}

				for (int j = 0; j < results[i].Count(); j++)
					result[j].Votes[metrics[i].Metric] = int.Parse(results[i][j]);
			}

			return result;
		}

		private Dictionary<string, Action<ElectionResultsDto, int>> electionResultsFieldParsers =
			new Dictionary<string, Action<ElectionResultsDto, int>>
			{
				{"Число избирателей, внесенных в список избирателей на момент окончания голосования",
					(e, v) => e.VotersCount = v},
				{"Число избирательных бюллетеней, полученных участковой избирательной комиссией",
					(e, v) => e.ReceivedBallotsCount = v},
				{"Число избирательных бюллетеней, выданных избирателям, проголосовавшим досрочно",
					(e, v) => e.EarlyIssuedBallotsCount = v},
				{"Число избирательных бюллетеней, выданных в помещении для голосования в день голосования",
					(e, v) => e.IssuedInsideBallotsCount = v},
				{"Число избирательных бюллетеней, выданных вне помещения для голосования в день голосования",
					(e, v) => e.IssuedOutsideBallotsCount = v},
				{"Число погашенных избирательных бюллетеней",
					(e, v) => e.CanceledBallotsCount = v},
				{"Число избирательных бюллетеней, содержащихся в переносных ящиках для голосования",
					(e, v) => e.OutsideBallotsCount = v},
				{"Число избирательных бюллетеней, содержащихся в стационарных ящиках для голосования",
					(e, v) => e.InsideBallotsCount = v},
				{"Число недействительных избирательных бюллетеней",
					(e, v) => e.InvalidBallotsCount = v},
				{"Число действительных избирательных бюллетеней",
					(e, v) => e.ValidBallotsCount = v},
				{"Число открепительных удостоверений, полученных участковой избирательной комиссией",
					(e, v) => e.ReceivedAbsenteeCertificatesCount = v},
				{"Число открепительных удостоверений, выданных на избирательном участке до дня голосования",
					(e, v) => e.IssuedAbsenteeCertificatesCount = v},
				{"Число избирателей, проголосовавших по открепительным удостоверениям на избирательном участке",
					(e, v) => e.AbsenteeCertificateVotersCount = v},
				{"Число погашенных неиспользованных открепительных удостоверений",
					(e, v) => e.CanceledAbsenteeCertificatesCount = v},
				{"Число открепительных удостоверений, выданных избирателям территориальной избирательной комиссией",
					(e, v) => e.IssuedByHigherDistrictAbsenteeCertificatesCount = v},
				{"Число утраченных открепительных удостоверений",
					(e, v) => e.LostAbsenteeCertificatesCount = v},
				{"Число утраченных избирательных бюллетеней",
					(e, v) => e.LostBallotsCount = v},
				{"Число избирательных бюллетеней, не учтенных при получении",
					(e, v) => e.UnaccountedBallotsCount = v},
			};
	}
}
