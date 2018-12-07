using System;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents information about a set value restriction.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the <see cref="SetRestrictionInfo{T}.Value"/> property.
    /// </typeparam>
    public class SetRestrictionInfo<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetRestrictionInfo{T}"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="minimumTimesAllowed">
        /// The minimum number of times the associated value must appear, if such a restriction exists.
        /// </param>
        /// <param name="maximumTimesAllowed">
        /// The maximum number of times the associated value is allowed to appear, if such a restriction exists.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="minimumTimesAllowed"/> is less than 0, <paramref name="maximumTimesAllowed"/>
        /// is less than 1, or <paramref name="maximumTimesAllowed"/> is less than
        /// <paramref name="minimumTimesAllowed"/>.
        /// </exception>
        public SetRestrictionInfo(
            T value,
            int? minimumTimesAllowed = null,
            int? maximumTimesAllowed = null)
        {
            this.Value = value;
            this.MaximumTimesAllowed = maximumTimesAllowed;
            this.MinimumTimesAllowed = minimumTimesAllowed;

            if (minimumTimesAllowed.HasValue && minimumTimesAllowed.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumTimesAllowed));
            }

            if (maximumTimesAllowed.HasValue
                && (maximumTimesAllowed.Value < 1 // If you don't want it to appear in the set, don't include it
                    || (minimumTimesAllowed.HasValue // Maximum count cannot be less than minimum count
                        && maximumTimesAllowed.Value < minimumTimesAllowed.Value)))
            {
                throw new ArgumentOutOfRangeException(nameof(maximumTimesAllowed));
            }
        }

        /// <summary>
        /// Gets the value associated with this restriction.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the maximum number of times the associated value is allowed to appear, if such a restriction exists.
        /// </summary>
        public int? MaximumTimesAllowed { get; }

        /// <summary>
        /// Gets the minimum number of times the associated value must appear, if such a restriction exists.
        /// </summary>
        public int? MinimumTimesAllowed { get; }
    }
}
