﻿using System;

namespace Drexel.Configurables.Contracts.Structs
{
    /// <summary>
    /// Represents information about a set value restriction for a <see langword="struct"/> type.
    /// </summary>
    /// <typeparam name="T">
    /// The <see langword="struct"/> type of the <see cref="StructSetRestrictionInfo{T}.Value"/> property.
    /// </typeparam>
    public sealed class StructSetRestrictionInfo<T> : SetRestrictionInfo
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructSetRestrictionInfo{T}"/> class.
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
        public StructSetRestrictionInfo(T value, int? minimumTimesAllowed = null, int? maximumTimesAllowed = null)
            : base(minimumTimesAllowed, maximumTimesAllowed)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the <see langword="struct"/> value associated with this restriction.
        /// </summary>
        public T Value { get; }
    }
}