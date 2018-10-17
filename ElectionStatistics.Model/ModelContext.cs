using Microsoft.EntityFrameworkCore;

namespace ElectionStatistics.Model
{
    public class ModelContext : DbContext
    {
        public ModelContext(DbContextOptions options) : base(options)
        {
            Candidates = new CandidatesRepository(this);
            Elections = new ElectionsRepository(this);
            ElectoralDistricts = new ElectoralDistrictsRepository(this);
            ElectoralDistrictElection = new ElectoralDistrictElectionsRepository(this);
            ElectionResults = new ElectionResultsRepository(this);
            ElectionCandidatesVotes = new ElectionCandidatesVotesRepository(this);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ElectionCandidate>()
                .HasKey(c => new { c.ElectionId, c.CandidateId });
            modelBuilder.Entity<ElectionCandidateVote>()
                .HasKey(c => new { c.ElectionResultId, c.CandidateId });
            modelBuilder.Entity<ElectoralDistrictElection>()
                .HasKey(c => new { c.ElectionId, c.ElectoralDistrictId });
        }

        public CandidatesRepository Candidates { get; private set; }
        public ElectionsRepository Elections { get; private set; }
        public ElectoralDistrictsRepository ElectoralDistricts { get; private set; }
        public ElectoralDistrictElectionsRepository ElectoralDistrictElection { get; private set; }
        public ElectionResultsRepository ElectionResults { get; private set; }
        public ElectionCandidatesVotesRepository ElectionCandidatesVotes { get; private set; }

        #region Список таблиц для автоматической миграции
        private DbSet<Candidate> CandidatesTable
        {
            get { return Set<Candidate>(); }
        }

        private DbSet<Election> ElectionsTable
        {
            get { return Set<Election>(); }
        }

        private DbSet<ElectoralDistrict> ElectoralDistrictsTable
        {
            get { return Set<ElectoralDistrict>(); }
        }

        private DbSet<ElectoralDistrictElection> ElectoralDistrictElectionTable
        {
            get { return Set<ElectoralDistrictElection>(); }
        }

        private DbSet<ElectionResult> ElectionResultsTable
        {
            get { return Set<ElectionResult>(); }
        }

        private DbSet<ElectionCandidateVote> ElectionCandidatesVotesTable
        {
            get { return Set<ElectionCandidateVote>(); }
        }

        private DbSet<LineCalculatedValue> LineCalculatedValues { get; set; }
        private DbSet<LineDescription> LineDescriptions { get; set; }
        private DbSet<LineNumber> LineNumbers { get; set; }
        private DbSet<LineString> LineStrings { get; set; }
        private DbSet<Mapping> Mappings { get; set; }
        private DbSet<MappingLine> MappingLines { get; set; }
        private DbSet<Protocol> Protocols { get; set; }
        private DbSet<ProtocolSet> ProtocolSets { get; set; }
        private DbSet<Preset> Presets { get; set; }
        #endregion
    }
}
