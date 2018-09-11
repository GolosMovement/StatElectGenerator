using System;
using Microsoft.EntityFrameworkCore;
using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Import
{
    public class DbSerializer : ISerializer
    {
        private DbContext context;
        private int commitCount = 0;
        private const int bulkSize = 4000;

        public DbSerializer(DbContext context)
        {
            this.context = context;
            context.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public void CreateProtocolSet(ProtocolSet protocolSet)
        {
            context.Set<ProtocolSet>().Add(protocolSet);
            context.SaveChanges();
        }

        public void CreateLineDescription(LineDescription lineDescription)
        {
            context.Set<LineDescription>().Add(lineDescription);
            context.SaveChanges();
        }

        public void CreateProtocol(Protocol protocol)
        {
            context.Set<Protocol>().Add(protocol);
            BulkSave();
        }

        public void CreateLineNumber(LineNumber lineNumber)
        {
            context.Set<LineNumber>().Add(lineNumber);
            BulkSave();
        }

        public void CreateLineString(LineString lineString)
        {
            context.Set<LineString>().Add(lineString);
            BulkSave();
        }

        public void AfterImport()
        {
            context.SaveChanges();
        }

        public void UpdateProtocolSet(ProtocolSet protocolSet)
        {
            context.Update(protocolSet);
            context.SaveChanges();
        }

        public void UpdateProtocolSetBulk(ProtocolSet protocolSet)
        {
            context.Update(protocolSet);
            BulkSave();
        }

        private void BulkSave()
        {
            commitCount++;

            if (commitCount % bulkSize == 0)
            {
                context.SaveChanges();
            }
        }
    }
}
