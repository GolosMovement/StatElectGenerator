using System;
using System.Linq;

namespace ElectionStatistics.Model
{
    public static class ElectoralDistrictElectionQueryableExtensions
    {
        public static IQueryable<ElectoralDistrictElection> ByElection(this IQueryable<ElectoralDistrictElection> items,
            int electionId)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Where(districtElection => districtElection.ElectionId == electionId);
        }
    }
}