using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    public class SetValidatorException : Exception
    {
        public SetValidatorException(string message)
            : this(message, null)
        {
            // Nothing to do.
        }

        public SetValidatorException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.HasValue = false;
        }

        private protected SetValidatorException(string message, object value)
            : base(message)
        {
            this.HasValue = true;
            this.Value = value;
        }

        public bool HasValue { get; }

        public object Value { get; }
    }
}
