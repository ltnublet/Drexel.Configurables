using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a validated set of requirements.
    /// </summary>
    public sealed class RequirementCollection : IReadOnlyCollection<IRequirement>
    {
        private readonly IReadOnlyCollection<IRequirement> backingCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementCollection"/> class.
        /// </summary>
        /// <param name="requirements">
        /// The requirements to initialize this collection with.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a supplied argument is illegally <see langword="null"/>.
        /// </exception>
        /// <exception cref="ConflictingRequirementsException">
        /// Thrown when the <paramref name="requirements"/> contains conflicting <see cref="IRequirement"/>s.
        /// </exception>
        /// <exception cref="DependenciesNotSatisfiedException">
        /// Thrown when the <paramref name="requirements"/> contains an <see cref="IRequirement"/> which does not
        /// have all dependencies met.
        /// </exception>
        /// <exception cref="DuplicateRequirementException">
        /// Thrown when the <paramref name="requirements"/> contains a duplicate <see cref="IRequirement"/>.
        /// </exception>
        public RequirementCollection(IReadOnlyCollection<IRequirement> requirements)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            HashSet<IRequirement> distinctRequirements = new HashSet<IRequirement>();
            foreach (IRequirement requirement in requirements)
            {
                if (distinctRequirements.Contains(requirement))
                {
                    throw new DuplicateRequirementException(requirement);
                }
            }

            ImmutableHashSet<IRequirement> asImmutable = distinctRequirements.ToImmutableHashSet();
            foreach (IRequirement requirement in requirements)
            {
                ImmutableHashSet<IRequirement> missing = asImmutable.Except(asImmutable);
                if (missing.Any())
                {
                    throw new DependenciesNotSatisfiedException(requirement, missing);
                }
            }

            foreach (IRequirement requirement in requirements)
            {
                ImmutableHashSet<IRequirement> conflicting = asImmutable.Intersect(asImmutable);
                if (conflicting.Any())
                {
                    throw new ConflictingRequirementsException(requirement, conflicting);
                }
            }

            this.backingCollection = distinctRequirements.ToArray();
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => this.backingCollection.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates through the collection.
        /// </returns>
        public IEnumerator<IRequirement> GetEnumerator() => this.backingCollection.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
