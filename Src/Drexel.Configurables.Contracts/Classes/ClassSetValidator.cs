using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables.Contracts.Classes
{
    /// <summary>
    /// Represents a set validator for a <see langword="class"/> type.
    /// </summary>
    /// <typeparam name="T">
    /// The <see langword="class"/> type of the set to validate.
    /// </typeparam>
    public sealed class ClassSetValidator<T> : SetValidator
        where T : class
    {
        private readonly IReadOnlyDictionary<T, ClassSetRestrictionInfo<T>>? backingSet;
        private readonly ClassSetRestrictionInfo<T>? nullRestriction;
        private readonly CollectionInfo? collectionInfo;
        private readonly ClassRequirementType<T> type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassSetValidator{T}"/> class.
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
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        /// <exception cref="DuplicateSetValueException">
        /// Thrown when <paramref name="restrictedToSet"/> illegally contains a duplicate
        /// <see cref="ClassSetRestrictionInfo{T}"/>.
        /// </exception>
        public ClassSetValidator(
            ClassRequirementType<T> type,
            IReadOnlyCollection<ClassSetRestrictionInfo<T>>? restrictedToSet = null,
            CollectionInfo? collectionInfo = null)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));

            Dictionary<T, ClassSetRestrictionInfo<T>>? buffer = null;
            if (restrictedToSet != null)
            {
                buffer = new Dictionary<T, ClassSetRestrictionInfo<T>>();
                foreach (ClassSetRestrictionInfo<T> restriction in restrictedToSet)
                {
                    if (restriction.Value == null)
                    {
                        if (this.nullRestriction == null)
                        {
                            this.nullRestriction = restriction;
                        }
                        else
                        {
                            throw new DuplicateSetValueException(null, typeof(ClassSetRestrictionInfo<T>));
                        }
                    }
                    else
                    {
                        if (buffer.ContainsKey(restriction.Value))
                        {
                            throw new DuplicateSetValueException(restriction.Value, typeof(ClassSetRestrictionInfo<T>));
                        }
                        else
                        {
                            buffer.Add(restriction.Value, restriction);
                        }
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
        /// <exception cref="AggregateException">
        /// Thrown when multiple <see cref="SetValidatorException"/>s occur.
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
        public void Validate(IEnumerable<T?> set)
        {
            SetValidatorException? ValidateCount(ClassSetRestrictionInfo<T> restriction, int timesSeen)
            {
                if (restriction.IsBelowRange(timesSeen))
                {
                    return new ValueBelowMinimumTimesThresholdException(
                        restriction.Value,
                        typeof(T),
                        timesSeen,
                        restriction.MinimumTimesAllowed.Value);
                }

                if (restriction.IsAboveRange(timesSeen))
                {
                    return new ValueAboveMaximumTimesThresholdException(
                        restriction.Value,
                        typeof(T),
                        timesSeen,
                        restriction.MaximumTimesAllowed.Value);
                }

                return null;
            }

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

            IEnumerator<T?> enumerator = set.GetEnumerator();
            int totalItems = 0;
            int timesNullSeen = 0;
            List<SetValidatorException> exceptions = new List<SetValidatorException>();
            Dictionary<T, int> timesSeenDictionary = this.backingSet.ToDictionary(x => x.Key, x => 0);
            bool @continue = true;
            while (@continue)
            {
                try
                {
                    @continue = enumerator.MoveNext();
                }
                catch (ValueOfWrongTypeException e)
                {
                    exceptions.Add(e);
                }

                T? current = enumerator.Current;

                if (current == null)
                {
                    if (this.nullRestriction == null)
                    {
                        exceptions.Add(new ValueNotInSetException(current, typeof(T)));
                    }
                    else
                    {
                        timesNullSeen++;
                    }
                }
                else
                {
                    if (this.backingSet.ContainsKey(current))
                    {
                        timesSeenDictionary[current]++;
                    }
                }

                totalItems++;
            }

            try
            {
                this.ValidateSetSize(totalItems);
            }
            catch (SetValidatorException e)
            {
                exceptions.Add(e);
            }

            foreach (T? value in set)
            {
                if (value == null)
                {
                    if (this.nullRestriction == null)
                    {
                        exceptions.Add(new ValueNotInSetException(value, typeof(T)));
                    }
                    else
                    {
                        timesNullSeen++;
                    }
                }
                else
                {
                    if (this.backingSet.TryGetValue(value, out ClassSetRestrictionInfo<T> restrictionInfo))
                    {
                        if (!timesSeenDictionary.ContainsKey(value))
                        {
                            timesSeenDictionary.Add(value, 0);
                        }

                        timesSeenDictionary[value]++;
                    }
                    else
                    {
                        exceptions.Add(new ValueNotInSetException(value, typeof(T)));
                    }
                }
            }

            if (this.nullRestriction != null)
            {
                SetValidatorException? e = ValidateCount(this.nullRestriction, timesNullSeen);
                if (e != null)
                {
                    exceptions.Add(e);
                }
            }

            foreach (KeyValuePair<T, int> value in timesSeenDictionary)
            {
                SetValidatorException? e = ValidateCount(this.backingSet[value.Key], value.Value);
                if (e != null)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count == 1)
            {
                throw exceptions[0];
            }
            else if (exceptions.Count > 1)
            {
                throw new AggregateException("Multiple exceptions occurred.", exceptions);
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
        /// <exception cref="AggregateException">
        /// Thrown when multiple <see cref="SetValidatorException"/>s occur.
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

            this.Validate(set.ToClassGenericEnumerable<T>(
                (object? x, out T? result) => this.type.TryCast(x, out result),
                (object? x) => throw new ValueOfWrongTypeException(x, typeof(T))));
        }
    }
}
