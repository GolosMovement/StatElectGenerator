using System;
using ElectionStatistics.Core.Exceptions;

namespace ElectionStatistics.Core.Preset
{
    public class ValidationException : BaseException
    {
        public ValidationException()
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
