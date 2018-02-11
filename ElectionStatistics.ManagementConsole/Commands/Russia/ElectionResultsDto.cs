using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionStatistics.ManagementConsole
{
	class ElectionResultsDto
	{
		public string ElectoralDistrictName;
		public Dictionary<string, int> Votes = new Dictionary<string, int>();

		/// <summary>
		/// Число избирателей, внесенных в список избирателей на момент окончания голосования
		/// </summary>
		public int VotersCount { get; set; }

		/// <summary>
		/// Число избирательных бюллетеней, полученных участковой избирательной комиссией
		/// </summary>	
		public int ReceivedBallotsCount { get; set; }

		/// <summary>
		/// Число избирательных бюллетеней, выданных избирателям, проголосовавшим досрочно
		/// </summary>	
		public int EarlyIssuedBallotsCount { get; set; }

		/// <summary>
		/// Число избирательных бюллетеней, выданных избирателям в помещении для голосования
		/// </summary>
		public int IssuedInsideBallotsCount { get; set; }

		/// <summary>
		/// Число избирательных бюллетеней, выданных избирателям вне помещения для голосования
		/// </summary>
		public int IssuedOutsideBallotsCount { get; set; }

		/// <summary>
		/// Число погашенных избирательных бюллетеней
		/// </summary>
		public int CanceledBallotsCount { get; set; }

		/// <summary>
		/// Число избирательных бюллетеней в переносных ящиках для голосования
		/// </summary>
		public int OutsideBallotsCount { get; set; }

		/// <summary>
		/// Число избирательных бюллетеней в стационарных ящиках для голосования
		/// </summary>	
		public int InsideBallotsCount { get; set; }

		/// <summary>
		/// Число недействительных избирательных бюллетеней
		/// </summary>
		public int InvalidBallotsCount { get; set; }

		/// <summary>
		/// Число действительных избирательных бюллетеней
		/// </summary>
		public int ValidBallotsCount { get; set; }

		/// <summary>
		/// Число открепительных удостоверений, полученных участковой избирательной комиссией
		/// </summary>
		public int ReceivedAbsenteeCertificatesCount { get; set; }

		/// <summary>
		/// Число открепительных удостоверений, выданных избирателям на избирательном участке
		/// </summary>	
		public int IssuedAbsenteeCertificatesCount { get; set; }

		/// <summary>
		/// Число избирателей, проголосовавших по открепительным удостоверениям на избирательном участке
		/// </summary>
		public int AbsenteeCertificateVotersCount { get; set; }

		/// <summary>
		/// Число погашенных неиспользованных открепительных удостоверений
		/// </summary>
		public int CanceledAbsenteeCertificatesCount { get; set; }

		/// <summary>
		/// Число открепительных удостоверений, выданных избирателям территориальной избирательной комиссией
		/// </summary>
		public int IssuedByHigherDistrictAbsenteeCertificatesCount { get; set; }

		/// <summary>
		/// Число утраченных открепительных удостоверений
		/// </summary>
		public int LostAbsenteeCertificatesCount { get; set; }

		/// <summary>
		/// Число утраченных избирательных бюллетеней
		/// </summary>
		public int LostBallotsCount { get; set; }

		/// <summary>
		/// Число избирательных бюллетеней, не учтенных при получении
		/// </summary>	
		public int UnaccountedBallotsCount { get; set; }
	}
}
