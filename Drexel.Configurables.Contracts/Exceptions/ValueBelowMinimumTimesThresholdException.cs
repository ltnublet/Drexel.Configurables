namespace Drexel.Configurables.Contracts.Exceptions
{
    public sealed class ValueBelowMinimumTimesThresholdException : SetValidatorException
    {
        public ValueBelowMinimumTimesThresholdException(object value, int timesSeen, int minimumTimes)
            : base("The supplied value is supplied a number of times below the minimum times threshold.", value)
        {
            this.MinimumTimes = minimumTimes;
            this.TimesSeen = timesSeen;
        }

        public int MinimumTimes { get; }

        public int TimesSeen { get; }
    }
}
