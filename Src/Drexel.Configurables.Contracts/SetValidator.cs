using System;
using System.Collections;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Performs set validation.
    /// </summary>
    public abstract class SetValidator
    {
        private readonly CollectionInfo? collectionInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetValidator"/> class.
        /// </summary>
        /// <param name="collectionInfo">
        /// A <see cref="CollectionInfo"/> representing information about the associated set, or <see langword="null"/>
        /// if no such information is applicable.
        /// </param>
        private protected SetValidator(CollectionInfo? collectionInfo = null)
        {
            this.collectionInfo = collectionInfo;
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
        public abstract void Validate(IEnumerable set);

        /// <summary>
        /// Validates the specified <paramref name="size"/> against the <see cref="CollectionInfo"/> supplied during
        /// instantiation if it was supplied.
        /// </summary>
        /// <param name="size"></param>
        /// <exception cref="MinimumCountNotMetException">
        /// Thrown when the supplied <paramref name="size"/> is below the minimum count of this set.
        /// </exception>
        /// <exception cref="MaximumCountExceededException">
        /// Thrown when the supplied <paramref name="size"/> is above the maximum count of this set.
        /// </exception>
        [System.Diagnostics.DebuggerHidden]
        protected void ValidateSetSize(int size)
        {
            if (this.collectionInfo != null)
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
}
