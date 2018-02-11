namespace ElectionStatistics.Model
{
    public class ElectionCandidatesVotesRepository : Repository<ElectionCandidateVote>
    {
        public ElectionCandidatesVotesRepository(ModelContext context) : base(context)
        {
        }
    }
}