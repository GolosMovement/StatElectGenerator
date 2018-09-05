using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
    [Table("ProtocolSets")]
    public class ProtocolSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string TitleRus { get; set; }
        public string TitleEng { get; set; }

        [Required]
        public string DescriptionRus { get; set; }
        public string DescriptionEng { get; set; }

        public string ImportFileErrorLog { get; set; }

        public virtual ICollection<Protocol> Protocols { get; set; }
    }
}
