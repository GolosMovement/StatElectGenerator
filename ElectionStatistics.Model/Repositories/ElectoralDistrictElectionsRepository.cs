using System.Linq;

namespace ElectionStatistics.Model
{
    public class ElectoralDistrictElectionsRepository : Repository<ElectoralDistrictElection>
    {
        public ElectoralDistrictElectionsRepository(ModelContext context) : base(context)
        {
        }

        public ElectoralDistrictElection GetOrAdd(Election election, ElectoralDistrict district, string dataSourceUrl)
        {
            var districtElection = this.SingleOrDefault(
                subRegionElection =>
                subRegionElection.ElectionId == election.Id &&
                subRegionElection.ElectoralDistrictId == district.Id);

            if (districtElection == null)
            {
                districtElection = new ElectoralDistrictElection
                {
                    Election = election,
                    ElectoralDistrict = district,
                    DataSourceUrl = dataSourceUrl
                };
                Context.ElectoralDistrictElection.Add(districtElection);
            }

            return districtElection;
        }
    }
}