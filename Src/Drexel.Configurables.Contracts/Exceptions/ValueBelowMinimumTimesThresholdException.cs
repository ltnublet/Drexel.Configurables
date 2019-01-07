using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The error that is thrown when a value appears fewer than the minimum times allowed threshold.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public sealed class ValueBelowMinimumTimesThresholdException : SetValidatorWithValueException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueBelowMinimumTimesThresholdException"/> class.
        /// </summary>
        /// <param name="value">
        /// The value associated with the error.
        /// </param>
        /// <param name="type">
        /// The type of the value associated with the error.
        /// </param>
        /// <param name="timesSeen">
        /// The actual number of times the value was seen.
        /// </param>
        /// <param name="minimumTimes">
        /// The threshold for the minimum number of times the value is allowed.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when a combination of arguments is illegal.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public ValueBelowMinimumTimesThresholdException(
            object? value,
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

            if (!(this.TimesSeen < this.MinimumTimes))
            {
                throw new ArgumentException("'timesSeen' must be less than 'minimumTimes'.", nameof(timesSeen));
            }
        }

        /// <summary>
        /// Gets the threshold for the minimum number of times the value is allowed.
        /// </summary>
        public int MinimumTimes { get; }

        /// <summary>
        /// Gets the number of times the value was seen.
        /// </summary>
        public int TimesSeen { get; }
    }
}
