using System;
using System.Linq;

namespace ElectionStatistics.Model
{
    public class ElectionsRepository : CachedRepository<Election>
    {
        public ElectionsRepository(ModelContext context)
            : base(context)
        {
        }

        public Election GetOrAdd(string name, ElectoralDistrict district, DateTime date, string dataSourceUrl)
        {
            var election =
                Cache.SingleOrDefault(e => e.Name == name && e.Date == date) ??
                this.SingleOrDefault(e => e.Name == name && e.Date == date);

            if (election == null)
            {
                election = new Election
                {
                    Name = name,
                    ElectoralDistrict = district,
                    Date = date,
                    DataSourceUrl = dataSourceUrl
                };
                Context.Elections.Add(election);
            }

            return election;
        }

        public Election GetById(int id)
        {
            return this.Single(election => election.Id == id);
        }
    }
}