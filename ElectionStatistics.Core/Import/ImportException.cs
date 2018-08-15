using System;

namespace ElectionStatistics.Core.Import
{
    // TODO: derive this exception from one custom base class
    public class ImportException : Exception
    {
        public ImportException()
        {
        }

        public ImportException(string message) : base(message)
        {
        }

        public ImportException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
