using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
    public enum MappingHierarchyType
    { 
        // Не является иерархией
        None,
        // Номер комиссии
        Number,
        // Название комиссии
        Name
    }

    [Table("MappingLines")]
    public class MappingLine
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int ColumnNumber { get; set; }

        public string TitleRus { get; set; }
        public string TitleEng { get; set; }
        public string TitleNative { get; set; }

        public string DescriptionRus { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionNative { get; set; }

        public bool IsNumber { get; set; }
        
        // Описывает ли строка результаты волеизъявления
        public bool IsVoteResult { get; set; }
        
        // Описывает ли строка результаты используемые в расчётах
        public bool IsCalcResult { get; set; }

        public MappingHierarchyType Hierarchy { get; set; }
        public int HierarchyLevel { get; set; }

        [Required]
        public int MappingId { get; set; }
        public virtual Mapping Mapping { get; set; }
    }
}
