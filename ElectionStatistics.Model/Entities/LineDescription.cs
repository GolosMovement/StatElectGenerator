using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
    [Table("LineDescriptions")]
    public class LineDescription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string TitleRus { get; set; }
        public string TitleEng { get; set; }
        public string TitleNative { get; set; }

        public string DescriptionRus { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionNative { get; set; }

        public bool IsVoteResult { get; set; }
        public bool IsCalcResult { get; set; }

        [Required]
        public int ProtocolSetId { get; set; }
        public virtual ProtocolSet ProtocolSet { get; set; }
    }
}
