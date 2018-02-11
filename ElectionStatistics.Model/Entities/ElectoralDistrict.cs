using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionStatistics.Model
{
	[Table("ElectoralDistricts")]
	public class ElectoralDistrict
    {
        public ElectoralDistrict()
        {
        }

        public ElectoralDistrict(string name, ElectoralDistrict higherDistrict)
        {
            Name = name;
            HigherDistrict = higherDistrict;
            HierarchyPath = higherDistrict == null 
                ? null 
                : higherDistrict.HierarchyPath == null
                    ? higherDistrict.Id.ToString()
                    : higherDistrict.HierarchyPath + "\\" + higherDistrict.Id;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Name { get; set; }

        public int? HigherDistrictId { get; set; }
        public virtual ElectoralDistrict HigherDistrict { get; set; }

        [MaxLength(100)]
        public string HierarchyPath { get; set; }
    }
}