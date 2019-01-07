namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the maximum count threshold is exceeded.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class MaximumCountExceededException : SetValidatorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaximumCountExceededException"/> class.
        /// </summary>
        /// <param name="maximumCount">
        /// The maximum count threshold that was exceeded.
        /// </param>
        /// <param name="actualCount">
        /// The actual count.
        /// </param>
        public MaximumCountExceededException(int maximumCount, int actualCount)
            : base("The maximum number of values was exceeded.")
        {
            this.ActualCount = actualCount;
            this.MaximumCount = maximumCount;
        }

        /// <summary>
        /// Gets the actual count.
        /// </summary>
        public int ActualCount { get; }

        /// <summary>
        /// Gets the maximum count threshold that was exceeded.
        /// </summary>
        public int MaximumCount { get; }
    }
}
