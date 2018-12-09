using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    public class SetValidatorException : Exception
    {
        public SetValidatorException()
        {
            // Nothing to do.
        }

        public SetValidatorException(string message)
            : base(message)
        {
            // Nothing to do.
        }

        public SetValidatorException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Nothing to do.
        }
    }
}
