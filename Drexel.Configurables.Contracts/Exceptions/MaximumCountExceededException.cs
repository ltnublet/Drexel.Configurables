namespace Drexel.Configurables.Contracts.Exceptions
{
    public class MaximumCountExceededException : SetValidatorException
    {
        public MaximumCountExceededException(int maximumCount, int actualCount)
            : base("The maximum number of values was exceeded.")
        {
            this.ActualCount = actualCount;
            this.MaximumCount = maximumCount;
        }

        public int ActualCount { get; }

        public int MaximumCount { get; }
    }
}
