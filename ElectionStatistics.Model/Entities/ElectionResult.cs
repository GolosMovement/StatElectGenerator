using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
	[Table("ElectionResults")]
	public class ElectionResult
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ElectoralDistrictId { get; set; }
        public virtual ElectoralDistrict ElectoralDistrict { get; set; }

        public int ElectionId { get; set; }
        public virtual Election Election { get; set; }

        public virtual ICollection<ElectionCandidateVote> Votes { get; set; }

        [Required, MaxLength(1000)]
        public string DataSourceUrl { get; set; }

        /// <summary>
        /// Число избирателей, внесенных в список избирателей
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