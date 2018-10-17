using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
    [Table("LineCalculatedValues")]
    public class LineCalculatedValue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public double Value { get; set; }

        public int? ProtocolId { get; set; }
        public virtual Protocol Protocol { get; set; }

        [Required]
        public int PresetId { get; set; }
        public virtual Preset Preset { get; set; }
    }
}
