using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
	[Table("Candidates")]
	public class Candidate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required, MaxLength(500)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string ShortName { get; set; }

        [Required, MaxLength(500)]
        public string GenitiveName { get; set; }

        public virtual ICollection<ElectionCandidate> Elections { get; set; }
    }
}