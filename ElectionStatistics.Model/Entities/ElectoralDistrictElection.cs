using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
	[Table("ElectoralDistrictElections")]
	public class ElectoralDistrictElection
	{
		[Required]
		public int ElectionId { get; set; }
        public virtual Election Election { get; set; }

		[Required]
		public int ElectoralDistrictId { get; set; }
        public virtual ElectoralDistrict ElectoralDistrict { get; set; }

        [Required, MaxLength(1000)]
        public string DataSourceUrl { get; set; }
    }
}