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
        private readonly IReadOnlyDictionary<T, SetRestrictionInfo<T>> backingSet;
        private readonly CollectionInfo? collectionInfo;

        public SetValidator(
            IReadOnlyCollection<SetRestrictionInfo<T>> restrictedToSet,
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
            if (this.backingSet == null)
            {
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
                    throw new ValueNotInSetException(value);
                }
            }

            int totalItems = timesSeenDictionary.Values.Sum();

            if (this.collectionInfo.HasValue)
            {
                if (this.collectionInfo.Value.MinimumCount.HasValue
                    && totalItems < this.collectionInfo.Value.MinimumCount.Value)
                {
                    throw new MinimumCountNotMetException();
                }

                if (this.collectionInfo.Value.MaximumCount.HasValue
                    && totalItems > this.collectionInfo.Value.MaximumCount.Value)
                {
                    throw new MaximumCountExceededException();
                }
            }

            foreach (KeyValuePair<T, SetRestrictionInfo<T>> restriction in this.backingSet)
            {
                bool wasSeen = timesSeenDictionary.TryGetValue(restriction.Key, out int timesSeen);
                if (restriction.Value.MinimumTimesAllowed.HasValue
                    && (!wasSeen || timesSeen < restriction.Value.MinimumTimesAllowed.Value))
                {
                    throw new ValueBelowMinimumTimesThresholdException(
                        restriction.Key,
                        timesSeen,
                        restriction.Value.MinimumTimesAllowed.Value);
                }

                if (restriction.Value.MinimumTimesAllowed.HasValue
                    && (!wasSeen || timesSeen > restriction.Value.MaximumTimesAllowed.Value))
                {
                    throw new ValueAboveMaximumTimesThresholdException(
                        restriction.Key,
                        timesSeen,
                        restriction.Value.MaximumTimesAllowed.Value);
                }
            }
        }

        public void Validate(IEnumerable set)
        {
            if (this.backingSet == null)
            {
                return;
            }

            try
            {
                this.Validate(set.Cast<T>());
            }
            catch (InvalidCastException e)
            {
                throw new SetValidatorException(
                    "Could not cast all elements of supplied set to expected type.",
                    e);
            }
        }
    }
}
