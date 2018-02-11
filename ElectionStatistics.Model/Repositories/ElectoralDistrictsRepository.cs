using System.Linq;

namespace ElectionStatistics.Model
{
    public class ElectoralDistrictsRepository : Repository<ElectoralDistrict>
    {
        public ElectoralDistrictsRepository(ModelContext context) : base(context)
        {
        }

        public ElectoralDistrict GetOrAdd(string name, ElectoralDistrict higherDistrict)
        {
            ElectoralDistrict district;
            if (higherDistrict == null)
            {
                district = this.SingleOrDefault(d =>
                    d.Name == name &&
                    d.HigherDistrictId == null);
            }
            else
            {
                district = this.SingleOrDefault(d =>
                    d.Name == name &&
                    d.HigherDistrictId == higherDistrict.Id);
            }

            if (district == null)
            {
                district = new ElectoralDistrict(name, higherDistrict);
                Add(district);
            }
            
            return district;
        }

        public ElectoralDistrict GetOrAddByUniqueName(string name)
        {
            var district = this.SingleOrDefault(d => d.Name == name);

            if (district == null)
            {
                district = new ElectoralDistrict
                {
                    Name = name,
                    HigherDistrict = null,
                    HierarchyPath = null
                };

                Add(district);
            }

            return district;
        }

		public void Import(ElectoralDistrict entity)
		{
			base.Add(entity);
		}


		public override void Add(ElectoralDistrict entity)
        {
            base.Add(entity);
            Context.SaveChanges();
        }
    }
}