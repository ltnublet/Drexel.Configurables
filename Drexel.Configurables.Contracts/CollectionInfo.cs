namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents information about a collection.
    /// </summary>
    public sealed class CollectionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionInfo"/> class.
        /// </summary>
        /// <param name="minimumCount">
        /// The minimum allowed number of values in the collection.
        /// </param>
        /// <param name="maximumCount">
        /// An optional maximum allowed number of values in the collection.
        /// </param>
        public CollectionInfo(int minimumCount, int? maximumCount = null)
        {
            this.MinimumCount = minimumCount;
            this.MaximumCount = maximumCount;
        }

        /// <summary>
        /// The minimum number of values allowed in the collection.
        /// </summary>
        public int MinimumCount { get; }

        /// <summary>
        /// The maximum number of values allowed in the collection. <see langword="null"/> if there is no maximum.
        /// </summary>
        public int? MaximumCount { get; }
    }
}
