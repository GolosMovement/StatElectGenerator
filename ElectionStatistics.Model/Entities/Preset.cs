using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
    [Table("Presets")]
    public class Preset
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Expression { get; set; }

        [Required]
        public string TitleRus { get; set; }
        public string TitleEng { get; set; }

        public string DescriptionRus { get; set; }
        public string DescriptionEng { get; set; }

        [Required]
        public int ProtocolSetId { get; set; }
        public virtual ProtocolSet ProtocolSet { get; set; }
    }
}
