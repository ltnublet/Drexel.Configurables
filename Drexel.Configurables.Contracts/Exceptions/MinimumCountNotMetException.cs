namespace Drexel.Configurables.Contracts.Exceptions
{
    public class MinimumCountNotMetException : SetValidatorException
    {
        public MinimumCountNotMetException()
            : base("The minimum number of values was not met.")
        {
            // Nothing to do.
        }
    }
}
