using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    public sealed class ValueNotInSetException : SetValidatorWithValueException
    {
        public ValueNotInSetException(object value, Type type)
            : base("The supplied value is not present in the allowed set of values.", value, type)
        {
            // Nothing to do.
        }
    }
}
