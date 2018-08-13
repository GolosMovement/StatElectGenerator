using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
    [Table("Protocols")]
    public class Protocol
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string TitleRus { get; set; }
        public string TitleEng { get; set; }
        public string TitleNative { get; set; }
        
        public int CommissionNumber { get; set; }

        [Required]
        public int ProtocolSetId { get; set; }
        public virtual ProtocolSet ProtocolSet { get; set; }

        public int? ParentId { get; set; }
        public virtual Protocol Parent { get; set; }

        public virtual ICollection<Protocol> Children { get; set; }

        public virtual ICollection<LineNumber> LineNumbers { get; set; }
        public virtual ICollection<LineString> LineStrings { get; set; }
    }
}
