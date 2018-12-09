using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    public class SetValidatorWithValueException : Exception
    {
        public SetValidatorWithValueException(string message, object value, Type type)
            : base(message)
        {
            this.Type = type;
            this.Value = value;
        }

        public Type Type { get; }

        public object Value { get; }
    }
}
