using log4net;

namespace ElectionStatistics.Core.Import
{
    // TODO: create and configure log4net logger especially for import errors
    public class ErrorLogger : IErrorLogger
    {
        private ILog log;

        public ErrorLogger(ILog log)
        {
            this.log = log;
        }

        public void Error(int line, int column, string humanColumn,
            string message)
        {
            log.ErrorFormat("{0}:{1} ({2}): {3}", line, column, humanColumn,
                message);
        }
    }
}
