using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    public class ValueOfWrongTypeException : SetValidatorException
    {
        public ValueOfWrongTypeException(
            object value,
            Type expectedType)
            : base("Supplied value is of the wrong type.")
        {
            this.Value = value;
            this.ExpectedType = expectedType;
        }

        public Type ExpectedType { get; }

        public object Value { get; }
    }
}
