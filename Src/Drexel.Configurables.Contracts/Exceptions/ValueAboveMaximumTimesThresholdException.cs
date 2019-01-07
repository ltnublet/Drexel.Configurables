using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The error that is thrown when a value appears more than the maximum times allowed threshold.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public sealed class ValueAboveMaximumTimesThresholdException : SetValidatorWithValueException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAboveMaximumTimesThresholdException"/> class.
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
        /// <param name="maximum">
        /// The threshold for the maximum number of times the value is allowed.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when a combination of arguments is illegal.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public ValueAboveMaximumTimesThresholdException(
            object? value,
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

            if (!(this.TimesSeen > this.MaximumTimes))
            {
                throw new ArgumentException("'timesSeen' must be greater than 'maximumTimes'.", nameof(timesSeen));
            }
        }

        /// <summary>
        /// Gets the threshold for the maximum number of times the value is allowed.
        /// </summary>
        public int MaximumTimes { get; }

        /// <summary>
        /// Gets the number of times the value was seen.
        /// </summary>
        public int TimesSeen { get; }
    }
}
