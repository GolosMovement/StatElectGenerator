using System;
using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Import
{
    // TODO: use in-memory storage instead of DB
    public class DbProgressNotifier : IProgressNotifier
    {
        private readonly DbSerializer serializer;
        private readonly ProtocolSet protocolSet;
        private DateTime? lastProgress;
        private const double updateFrequencySeconds = 15.0;

        public DbProgressNotifier(DbSerializer serializer,
            ProtocolSet protocolSet)
        {
            this.serializer = serializer;
            this.protocolSet = protocolSet;
        }

        public void Start(int totalLines)
        {
            protocolSet.ImportStartedAt = DateTime.Now;
            protocolSet.ImportFinishedAt = null;

            protocolSet.ImportCurrentLine = 0;
            protocolSet.ImportTotalLines = totalLines;
            protocolSet.ImportSuccess = false;

            serializer.UpdateProtocolSet(protocolSet);
        }

        public void Progress(int currentLine, int errorCount)
        {
            if (lastProgress == null ||
                (DateTime.Now - lastProgress.Value).TotalSeconds >=
                    updateFrequencySeconds)
            {
                SaveProgress(currentLine, errorCount);
            }
        }

        public void Finish(int currentLine, bool success, int errorCount)
        {
            protocolSet.ImportFinishedAt = DateTime.Now;
            protocolSet.ImportCurrentLine = currentLine;
            protocolSet.ImportSuccess = success;
            protocolSet.ImportErrorCount = errorCount;

            serializer.UpdateProtocolSet(protocolSet);
        }

        private void SaveProgress(int currentLine, int errorCount)
        {
            protocolSet.ImportCurrentLine = currentLine;
            protocolSet.ImportErrorCount = errorCount;

            serializer.UpdateProtocolSet(protocolSet);

            lastProgress = DateTime.Now;
        }
    }
}