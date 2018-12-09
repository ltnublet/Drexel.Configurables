namespace Drexel.Configurables.Contracts.Exceptions
{
    public class MinimumCountNotMetException : SetValidatorException
    {
        public MinimumCountNotMetException(int minimumCount, int actualCount)
            : base("The minimum number of values was not met.")
        {
            this.ActualCount = actualCount;
            this.MinimumCount = minimumCount;
        }

        public int ActualCount { get; }

        public int MinimumCount { get; }
    }
}
