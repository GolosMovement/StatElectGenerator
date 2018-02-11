using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
	[Table("Elections")]
	public class Election
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public DateTime Date { get; set; }

        [Required, MaxLength(1000)]
        public string DataSourceUrl { get; set; }

        public int ElectoralDistrictId { get; set; }
        public virtual ElectoralDistrict ElectoralDistrict { get; set; }

        public virtual ICollection<ElectionCandidate> Candidates { get; set; }
    }
}