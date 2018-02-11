namespace ElectionStatistics.Model
{
    public class ElectionResultsRepository : Repository<ElectionResult>
    {
        public ElectionResultsRepository(ModelContext context)
            : base(context)
        {
        }
    }
}