using System;
using ElectionStatistics.Core.Exceptions;

namespace ElectionStatistics.Core.Import
{
    public class ImportException : BaseException
    {
        public ImportException()
        {
        }

        public ImportException(string message)
            : base(message)
        {
        }

        public ImportException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
