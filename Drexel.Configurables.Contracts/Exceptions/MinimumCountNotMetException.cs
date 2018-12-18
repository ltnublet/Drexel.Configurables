namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the minimum count threshold is not met.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class MinimumCountNotMetException : SetValidatorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinimumCountNotMetException"/> class.
        /// </summary>
        /// <param name="minimumCount">
        /// The minimum count threshold that was not met.
        /// </param>
        /// <param name="actualCount">
        /// The actual count.
        /// </param>
        public MinimumCountNotMetException(int minimumCount, int actualCount)
            : base("The minimum number of values was not met.")
        {
            this.ActualCount = actualCount;
            this.MinimumCount = minimumCount;
        }

        /// <summary>
        /// Gets the actual count.
        /// </summary>
        public int ActualCount { get; }

        /// <summary>
        /// Gets the minimum count threshold that was not met.
        /// </summary>
        public int MinimumCount { get; }
    }
}
