using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
    [Table("LineStrings")]
    public class LineString
    {
        // TODO: use long type
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int? ProtocolId { get; set; }
        public virtual Protocol Protocol { get; set; }

        [Required]
        public int LineDescriptionId { get; set; }
        public virtual LineDescription LineDescription { get; set; }
    }
}
