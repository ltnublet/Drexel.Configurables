using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    public sealed class ValueBelowMinimumTimesThresholdException : SetValidatorWithValueException
    {
        public ValueBelowMinimumTimesThresholdException(
            object value,
            Type type,
            int timesSeen,
            int minimumTimes)
            : base(
                  "The supplied value is supplied a number of times below the minimum times threshold.",
                  value,
                  type)
        {
            this.MinimumTimes = minimumTimes;
            this.TimesSeen = timesSeen;
        }

        public int MinimumTimes { get; }

        public int TimesSeen { get; }
    }
}
