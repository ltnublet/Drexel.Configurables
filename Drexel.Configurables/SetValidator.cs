using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables
{
    public class SetValidator<T> : ISetValidator<T>
    {
        private readonly IReadOnlyDictionary<T, SetRestrictionInfo<T>>? backingSet;
        private readonly CollectionInfo? collectionInfo;

        public SetValidator(
            IReadOnlyCollection<SetRestrictionInfo<T>>? restrictedToSet = null,
            CollectionInfo? collectionInfo = null)
        {
            try
            {
                this.backingSet = restrictedToSet?.ToDictionary(x => x.Value, x => x);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Set must not contain duplicate values.", nameof(restrictedToSet));
            }

            this.collectionInfo = collectionInfo;
        }

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
                if (this.backingSet.TryGetValue(value, out SetRestrictionInfo<T> restrictionInfo))
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

            foreach (KeyValuePair<T, SetRestrictionInfo<T>> restriction in this.backingSet)
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

        public void Validate(IEnumerable set)
        {
            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            if (this.backingSet == null)
            {
                if (this.collectionInfo == null)
                {
                    return;
                }
                else
                {
                    IEnumerator enumerator = set.GetEnumerator();
                    int setSize;
                    for (setSize = 0; enumerator.MoveNext(); setSize++)
                    {
                    }

                    this.ValidateSetSize(setSize);
                }
            }

            this.Validate(SetValidator<T>.ToGenericEnumerable(set));
        }

        [System.Diagnostics.DebuggerHidden]
        private static IEnumerable<T> ToGenericEnumerable(IEnumerable nonGeneric)
        {
            IEnumerator enumerator = nonGeneric.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is T asT)
                {
                    yield return asT;
                }
                else
                {
                    throw new ValueOfWrongTypeException(enumerator.Current, typeof(T));
                }
            }
        }

        [System.Diagnostics.DebuggerHidden]
        private void ValidateSetSize(int size)
        {
            if (this.collectionInfo.Value.IsTooSmall(size))
            {
                throw new MinimumCountNotMetException(
                    this.collectionInfo.Value.MinimumCount.Value,
                    size);
            }

            if (this.collectionInfo.Value.IsTooLarge(size))
            {
                throw new MaximumCountExceededException(
                    this.collectionInfo.Value.MaximumCount.Value,
                    size);
            }
        }
    }
}
