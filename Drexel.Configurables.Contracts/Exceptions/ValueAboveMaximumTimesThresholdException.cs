namespace Drexel.Configurables.Contracts.Exceptions
{
    public sealed class ValueAboveMaximumTimesThresholdException : SetValidatorException
    {
        public ValueAboveMaximumTimesThresholdException(object value, int timesSeen, int maximum)
            : base("The supplied value is supplied a number of times above the maximum times threshold.", value)
        {
            this.MaximumTimes = maximum;
            this.TimesSeen = timesSeen;
        }

        public int MaximumTimes { get; }

        public int TimesSeen { get; }
    }
}
