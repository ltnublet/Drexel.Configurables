namespace Drexel.Configurables.Contracts.Exceptions
{
    public sealed class ValueNotInSetException : SetValidatorException
    {
        public ValueNotInSetException(object value)
            : base("The supplied value is not present in the allowed set of values.", value)
        {
            // Nothing to do.
        }
    }
}
