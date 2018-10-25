using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ElectionStatistics.Model;

using EFCore.BulkExtensions;

namespace ElectionStatistics.Core.Import
{
    // TODO: tests
    // FIXME: possible PK conflicts due to client side ids generation
    public class DbSerializer : ISerializer
    {
        private DbContext context;

        private const int defaultId = 0;
        private int protocolId;
        private int lineNumberId;
        private int lineStringId;

        private List<Protocol> protocols;
        private List<LineNumber> lineNumbers;
        private List<LineString> lineStrings;

        private int chunkSize = 1000000;

        public DbSerializer(DbContext context)
        {
            this.context = context;
        }

        public void BeforeImport()
        {
            CreateLists();
            LoadIdentities();
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
            protocol.Id = NextProtocolId();
            protocols.Add(protocol);
        }

        public void CreateLineNumber(LineNumber lineNumber)
        {
            lineNumber.Id = NextLineNumberId();
            lineNumbers.Add(lineNumber);
        }

        public void CreateLineString(LineString lineString)
        {
            lineString.Id = NextLineStringId();
            lineStrings.Add(lineString);
        }

        public void AfterImport()
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                context.BulkInsert(protocols);

                if (lineNumbers.Count <= chunkSize)
                {
                    context.BulkInsert(lineNumbers);
                }

                context.BulkInsert(lineStrings);
                transaction.Commit();
            }

            if (lineNumbers.Count > chunkSize)
            {
                var chunkOffset = 0;

                while (chunkOffset < lineNumbers.Count)
                {
                    var lineNumbersChunk =
                        lineNumbers.Skip(chunkOffset).Take(chunkSize).ToList();

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        context.BulkInsert(lineNumbersChunk);
                        transaction.Commit();
                    }

                    chunkOffset += lineNumbersChunk.Count;
                }

            }
        }

        public void UpdateProtocolSet(ProtocolSet protocolSet)
        {
            context.Update(protocolSet);
            context.SaveChanges();
        }

        private void LoadIdentities()
        {
            LoadProtocolId();
            LoadLineNumberId();
            LoadLineStringId();
        }

        // TODO: DRY
        private void LoadProtocolId()
        {
            var obj = context.Set<Protocol>()
                .OrderByDescending(o => o.Id)
                .FirstOrDefault();

            protocolId = obj == null ? defaultId : obj.Id;
        }

        private void LoadLineNumberId()
        {
            var obj = context.Set<LineNumber>()
                .OrderByDescending(o => o.Id)
                .FirstOrDefault();

            lineNumberId = obj == null ? defaultId : obj.Id;
        }

        private void LoadLineStringId()
        {
            var obj = context.Set<LineString>()
                .OrderByDescending(o => o.Id)
                .FirstOrDefault();

            lineStringId = obj == null ? defaultId : obj.Id;
        }

        private int NextProtocolId()
        {
            return ++protocolId;
        }

        private int NextLineNumberId()
        {
            return ++lineNumberId;
        }

        private int NextLineStringId()
        {
            return ++lineStringId;
        }

        private void CreateLists()
        {
            this.protocols = new List<Protocol>();
            this.lineNumbers = new List<LineNumber>();
            this.lineStrings = new List<LineString>();
        }
    }
}
