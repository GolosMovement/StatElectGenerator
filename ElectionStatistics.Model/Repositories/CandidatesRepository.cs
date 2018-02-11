using System.Collections.ObjectModel;
using System.Linq;

namespace ElectionStatistics.Model
{
    public class CandidatesRepository : CachedRepository<Candidate>
    {
        public CandidatesRepository(ModelContext context) : base(context)
        {
        }

        public Candidate GetOrAdd(string name, Election election)
        {
            var candidate = Cache.SingleOrDefault(c => c.Name == name) ?? this.SingleOrDefault(c => c.Name == name);

            if (candidate == null)
            {
                candidate = new Candidate
                {
                    Name = name,
                    ShortName = "ХЗ",
                    GenitiveName = name,
                    Elections = new Collection<ElectionCandidate>()
                };
                Add(candidate);
            }

            if (!candidate.Elections.Any(e => e.Election == election))
            {
                candidate.Elections.Add(new ElectionCandidate
                {
	                Candidate = candidate,
					Election = election
				});
            }

            return candidate;
        }
    }
}