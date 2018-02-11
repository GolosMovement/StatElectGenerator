using System;
using System.Linq;

namespace ElectionStatistics.Model
{
    public static class ElectionCandidatesVotesQueryableExtensions
    {
        public static IQueryable<ElectionCandidateVote> ByCandidate(
            this IQueryable<ElectionCandidateVote> items,
            Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            return items.ByCandidate(candidate.Id);
        }

        public static IQueryable<ElectionCandidateVote> ByCandidate(
            this IQueryable<ElectionCandidateVote> items,
            int candidateId)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Where(result => result.CandidateId == candidateId);
        } 
    }
}