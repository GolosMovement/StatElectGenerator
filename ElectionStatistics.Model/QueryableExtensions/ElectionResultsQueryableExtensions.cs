using System;
using System.Linq;

namespace ElectionStatistics.Model
{
    public static class ElectionResultsQueryableExtensions
    {
        public static IQueryable<ElectionResult> ByHigherDistrict(
            this IQueryable<ElectionResult> items,
            ElectoralDistrict district)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            var childrenHierarchyPath = district.GetChildrenHierarchyPath();
            return items
                .Where(electionResult => electionResult.ElectoralDistrict.HierarchyPath.StartsWith(childrenHierarchyPath));
        }

        public static IQueryable<ElectionResult> ByElection(
            this IQueryable<ElectionResult> items, 
            int electionId)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Where(result => result.ElectionId == electionId);
        }
    }
}