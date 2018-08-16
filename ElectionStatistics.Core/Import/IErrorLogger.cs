namespace ElectionStatistics.Core.Import
{
    public interface IErrorLogger
    {
        void Error(int line, int column, string humanColumn, string message);
    }
}
