using System;
using System.Linq;

namespace ElectionStatistics.Model
{
    public static class ElectoralDistrictQueryableExtensions
    {
        public static IQueryable<ElectoralDistrict> ByHigherDistrict(
            this IQueryable<ElectoralDistrict> items,
            int higherDistrictId)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Where(district => district.HigherDistrictId == higherDistrictId);
        }

        public static IQueryable<ElectoralDistrict> ByElection(
            this IQueryable<ElectoralDistrict> items, 
            ModelContext context,
            int electionId)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (context == null)
                throw new ArgumentNullException("context");

            return context.ElectoralDistrictElection
                .ByElection(electionId)
                .Join(
                    items,
                    districtElection => districtElection.ElectoralDistrictId,
                    district => district.Id,
                    (election, district) => district);
        }

        public static ElectoralDistrict GetById(this IQueryable<ElectoralDistrict> items, int districtId)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.First(district => district.Id == districtId);
        }
    }
}