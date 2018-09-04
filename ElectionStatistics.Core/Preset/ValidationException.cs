using System;

namespace ElectionStatistics.Core.Preset
{
    // TODO: derive this exception from one custom base class
    public class ValidationException : Exception
    {
        public ValidationException()
        {
        }

        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception inner) :
            base(message, inner)
        {
        }
    }
}
