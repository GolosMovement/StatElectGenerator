using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
    [Table("MappingLines")]
    public class MappingLine
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        // One-based index
        [Required]
        public int ColumnNumber { get; set; }

        public string TitleRus { get; set; }
        public string TitleEng { get; set; }
        public string TitleNative { get; set; }

        public string DescriptionRus { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionNative { get; set; }

        public bool IsHierarchy { get; set; }
        // 1 - is the root, 2 - the second level, and so on
        public int HierarchyLevel { get; set; }

        public bool IsNumber { get; set; }
        
        // "Описывает ли строка результаты волеизъявления"
        public bool IsVoteResult { get; set; }
        
        // "Описывает ли строка результаты используемые в расчётах"
        public bool IsCalcResult { get; set; }

        [Required]
        public int MappingId { get; set; }
        public virtual Mapping Mapping { get; set; }
    }
}
