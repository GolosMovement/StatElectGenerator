namespace ElectionStatistics.Core.Import
{
    public interface IErrorLogger
    {
        string GetFileName();
        int GetErrorsCount();
        void Error(int line, int column, string humanColumn, string message);
    }
}
