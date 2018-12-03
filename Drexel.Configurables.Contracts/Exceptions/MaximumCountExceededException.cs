namespace Drexel.Configurables.Contracts.Exceptions
{
    public class MaximumCountExceededException : SetValidatorException
    {
        public MaximumCountExceededException()
            : base("The maximum number of values was exceeded.")
        {
            // Nothing to do.
        }
    }
}
