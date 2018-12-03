using System;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents information about a collection.
    /// </summary>
    public struct CollectionInfo : IEquatable<CollectionInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionInfo"/> struct.
        /// </summary>
        /// <param name="minimumCount">
        /// The minimum number of elements allowed in the collection, if a limit exists.
        /// </param>
        /// <param name="maximumCount">
        /// The maximum number of elements allowed in the collection, if a limit exists.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="minimumCount"/> is less than 0, or <paramref name="maximumCount"/> is specified
        /// but less than <paramref name="minimumCount"/>.
        /// </exception>
        public CollectionInfo(
            int? minimumCount = null,
            int? maximumCount = null)
        {
            if (minimumCount.HasValue && minimumCount.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumCount));
            }

            if (maximumCount.HasValue && maximumCount.Value < minimumCount)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCount));
            }

            this.MinimumCount = minimumCount;
            this.MaximumCount = maximumCount;
        }

        /// <summary>
        /// Gets the minimum number of elements allowed in the collection, if a limit exists.
        /// </summary>
        public int? MinimumCount { get; }

        /// <summary>
        /// Gets the maximum number of elements allowed in the collection, if a limit exists.
        /// </summary>
        public int? MaximumCount { get; }

        /// <summary>
        /// Determines whether the specified <see cref="CollectionInfo"/>s are equal.
        /// </summary>
        /// <param name="left">
        /// The first <see cref="CollectionInfo"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="CollectionInfo"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified <see cref="CollectionInfo"/> is equal to the current
        /// <see cref="CollectionInfo"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(CollectionInfo left, CollectionInfo right) => left.Equals(right);

        /// <summary>
        /// Determines whether the specified <see cref="CollectionInfo"/>s are not equal.
        /// </summary>
        /// <param name="left">
        /// The first <see cref="CollectionInfo"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="CollectionInfo"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified <see cref="CollectionInfo"/> is not equal to the current
        /// <see cref="CollectionInfo"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(CollectionInfo left, CollectionInfo right) => !(left == right);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified object is equal to the current object; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is CollectionInfo other)
            {
                return this.Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="CollectionInfo"/> is equal to the current
        /// <see cref="CollectionInfo"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="CollectionInfo"/> to compare with the current <see cref="CollectionInfo"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified <see cref="CollectionInfo"/> is equal to the current
        /// <see cref="CollectionInfo"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(CollectionInfo other)
        {
            return other.MinimumCount == this.MinimumCount
                && other.MaximumCount == this.MaximumCount;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// The hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 27;
                hash = (hash * 13) + this.MinimumCount.GetHashCode();
                hash = (hash * 13) + this.MaximumCount.GetHashCode();

                return hash;
            }
        }
    }
}
