using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    public sealed class ValueAboveMaximumTimesThresholdException : SetValidatorWithValueException
    {
        public ValueAboveMaximumTimesThresholdException(
            object value,
            Type type,
            int timesSeen,
            int maximum)
            : base(
                  "The supplied value is supplied a number of times above the maximum times threshold.",
                  value,
                  type)
        {
            this.MaximumTimes = maximum;
            this.TimesSeen = timesSeen;
        }

        public int MaximumTimes { get; }

        public int TimesSeen { get; }
    }
}
