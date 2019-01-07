using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables.Contracts.Structs
{
    /// <summary>
    /// Represents a set validator for a <see langword="struct"/> type.
    /// </summary>
    /// <typeparam name="T">
    /// The <see langword="struct"/> type of the set to validate.
    /// </typeparam>
    public sealed class StructSetValidator<T> : SetValidator
        where T : struct
    {
        private readonly StructRequirementType<T> type;
        private readonly IReadOnlyDictionary<T, StructSetRestrictionInfo<T>>? backingSet;
        private readonly CollectionInfo? collectionInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructSetValidator{T}"/> class.
        /// </summary>
        /// <param name="type">
        /// The <see cref="RequirementType"/> to use when interacting with values supplied to this validator.
        /// </param>
        /// <param name="restrictedToSet">
        /// The set of values that the validator is restricted to (and any count restrictions associated with an
        /// individual value), or <see langword="null"/> if no such restrictions exist.
        /// </param>
        /// <param name="collectionInfo">
        /// The size restrictions on the overall set, or <see langword="null"/> if no such restrictions exist.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally null.
        /// </exception>
        /// <exception cref="DuplicateSetValueException">
        /// Thrown when <paramref name="restrictedToSet"/> illegally contains a duplicate
        /// <see cref="StructSetRestrictionInfo{T}"/>.
        /// </exception>
        public StructSetValidator(
            StructRequirementType<T> type,
            IReadOnlyCollection<StructSetRestrictionInfo<T>>? restrictedToSet = null,
            CollectionInfo? collectionInfo = null)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));

            Dictionary<T, StructSetRestrictionInfo<T>>? buffer = null;
            if (restrictedToSet != null)
            {
                buffer = new Dictionary<T, StructSetRestrictionInfo<T>>();
                foreach (StructSetRestrictionInfo<T> restriction in restrictedToSet)
                {
                    if (buffer.ContainsKey(restriction.Value))
                    {
                        throw new DuplicateSetValueException(restriction.Value, typeof(StructSetRestrictionInfo<T>));
                    }
                    else
                    {
                        buffer.Add(restriction.Value, restriction);
                    }
                }
            }

            this.backingSet = buffer;
            this.collectionInfo = collectionInfo;
        }

        /// <summary>
        /// Validates the supplied set.
        /// </summary>
        /// <param name="set">
        /// The set to validate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="set"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="MinimumCountNotMetException">
        /// Thrown when <paramref name="set"/> does not meet the minimum size requirement.
        /// </exception>
        /// <exception cref="MaximumCountExceededException">
        /// Thrown when <paramref name="set"/> exceeds the maxmimum size requirement.
        /// </exception>
        /// <exception cref="ValueNotInSetException">
        /// Thrown when <paramref name="set"/> contains a value that is not in the set of values this validator is
        /// restricted to.
        /// </exception>
        /// <exception cref="ValueBelowMinimumTimesThresholdException">
        /// Thrown when <paramref name="set"/> contains a value too few times.
        /// </exception>
        /// <exception cref="ValueAboveMaximumTimesThresholdException">
        /// Thrown when <paramref name="set"/> contains a value too many times.
        /// </exception>
        public void Validate(IEnumerable<T> set)
        {
            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            if (this.backingSet == null)
            {
                if (this.collectionInfo != null)
                {
                    this.ValidateSetSize(set.Count());
                }

                return;
            }

            Dictionary<T, int> timesSeenDictionary = new Dictionary<T, int>(this.backingSet.Count);
            foreach (T value in set)
            {
                if (this.backingSet.TryGetValue(value, out StructSetRestrictionInfo<T> restrictionInfo))
                {
                    if (!timesSeenDictionary.ContainsKey(value))
                    {
                        timesSeenDictionary.Add(value, 0);
                    }

                    timesSeenDictionary[value]++;
                }
                else
                {
                    throw new ValueNotInSetException(value, typeof(T));
                }
            }

            int totalItems = timesSeenDictionary.Values.Sum();
            if (this.collectionInfo.HasValue)
            {
                this.ValidateSetSize(totalItems);
            }

            foreach (KeyValuePair<T, StructSetRestrictionInfo<T>> restriction in this.backingSet)
            {
                if (!timesSeenDictionary.TryGetValue(restriction.Key, out int timesSeen))
                {
                    timesSeen = 0;
                }

                if (restriction.Value.IsBelowRange(timesSeen))
                {
                    throw new ValueBelowMinimumTimesThresholdException(
                        restriction.Key,
                        typeof(T),
                        timesSeen,
                        restriction.Value.MinimumTimesAllowed.Value);
                }

                if (restriction.Value.IsAboveRange(timesSeen))
                {
                    throw new ValueAboveMaximumTimesThresholdException(
                        restriction.Key,
                        typeof(T),
                        timesSeen,
                        restriction.Value.MaximumTimesAllowed.Value);
                }
            }
        }

        /// <summary>
        /// Validates the supplied set.
        /// </summary>
        /// <param name="set">
        /// The set to validate.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="set"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValueOfWrongTypeException">
        /// Thrown when a value contained by <paramref name="set"/> is not of the <see cref="Type"/> this set validator
        /// expects.
        /// </exception>
        /// <exception cref="MinimumCountNotMetException">
        /// Thrown when <paramref name="set"/> does not meet the minimum size requirement.
        /// </exception>
        /// <exception cref="MaximumCountExceededException">
        /// Thrown when <paramref name="set"/> exceeds the maxmimum size requirement.
        /// </exception>
        /// <exception cref="ValueNotInSetException">
        /// Thrown when <paramref name="set"/> contains a value that is not in the set of values this validator is
        /// restricted to.
        /// </exception>
        /// <exception cref="ValueBelowMinimumTimesThresholdException">
        /// Thrown when <paramref name="set"/> contains a value too few times.
        /// </exception>
        /// <exception cref="ValueAboveMaximumTimesThresholdException">
        /// Thrown when <paramref name="set"/> contains a value too many times.
        /// </exception>
        public override void Validate(IEnumerable set)
        {
            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            if (this.backingSet == null)
            {
                if (this.collectionInfo != null)
                {
                    IEnumerator enumerator = set.GetEnumerator();
                    int setSize;
                    for (setSize = 0; enumerator.MoveNext(); setSize++)
                    {
                    }

                    this.ValidateSetSize(setSize);
                }

                return;
            }

            this.Validate(set.ToStructGenericEnumerable<T>(
                (object? x, out T result) => this.type.TryCast(x, out result),
                (object? x) => throw new ValueOfWrongTypeException(x, typeof(T))));
        }
    }
}
