namespace ElectionStatistics.Core.Import
{
    public interface IProgressNotifier
    {
        void Start(int totalLines);
        void Progress(int currentLine, int errorCount);
        void Finish(int currentLine, bool success, int errorCount);
    }
}