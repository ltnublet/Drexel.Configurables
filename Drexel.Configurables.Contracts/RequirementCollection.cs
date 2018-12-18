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
        /// <exception cref="AggregateException">
        /// Occurs if multiple validation exceptions occur during initialization of this collection.
        /// </exception>
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

            List<Exception> exceptions = new List<Exception>();
            HashSet<IRequirement> distinctRequirements = new HashSet<IRequirement>();
            foreach (IRequirement requirement in requirements)
            {
                if (distinctRequirements.Contains(requirement))
                {
                    exceptions.Add(new DuplicateRequirementException(requirement));
                }
                else
                {
                    distinctRequirements.Add(requirement);
                }
            }

            ImmutableHashSet<IRequirement> asImmutable = distinctRequirements.ToImmutableHashSet();
            foreach (IRequirement requirement in distinctRequirements)
            {
                ImmutableHashSet<IRequirement> conflicting = asImmutable.Intersect(asImmutable);
                if (conflicting.Any())
                {
                    exceptions.Add(new ConflictingRequirementsException(requirement, conflicting));
                    distinctRequirements.Remove(requirement);
                }
            }

            asImmutable = distinctRequirements.ToImmutableHashSet();
            foreach (IRequirement requirement in distinctRequirements)
            {
                ImmutableHashSet<IRequirement> missing = asImmutable.Except(asImmutable);
                if (missing.Any())
                {
                    exceptions.Add(new DependenciesNotSatisfiedException(requirement, missing));
                }
            }

            if (exceptions.Count == 1)
            {
                throw exceptions[0];
            }
            else if (exceptions.Count > 1)
            {
                throw new AggregateException("Multiple validation failures occurred.", exceptions);
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
