using System;
using System.Linq;

namespace ElectionStatistics.Model
{
    public static class CandidatesQueryableExtensions
    {
        public static IQueryable<Candidate> ByElection(
            this IQueryable<Candidate> items,
            ModelContext context,
            int electionId)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Where(candidate => candidate.Elections.Any(election => election.ElectionId == electionId));
        }

        public static Candidate GetById(this IQueryable<Candidate> items, int candidateId)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.First(candidate => candidate.Id == candidateId);
        }
    }
}