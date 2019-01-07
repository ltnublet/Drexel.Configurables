using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a requirement does not have all its dependencies satisfied.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public sealed class DependenciesNotSatisfiedException : RequirementSetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependenciesNotSatisfiedException"/> class.
        /// </summary>
        /// <param name="requirement">
        /// The requirement which did not have all dependencies satisfied.
        /// </param>
        /// <param name="missingRequirements">
        /// The set of requirements that the requirement depended on that were missing.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public DependenciesNotSatisfiedException(
            Requirement requirement,
            IReadOnlyCollection<Requirement> missingRequirements)
            : base("Specified requirement set does not satisfy dependencies for specified requirement.")
        {
            this.Requirement = requirement
                ?? throw new ArgumentNullException(nameof(requirement));
            this.MissingRequirements = missingRequirements
                ?? throw new ArgumentNullException(nameof(missingRequirements));
        }

        /// <summary>
        /// Gets the requirement that did not have all dependencies satisfied.
        /// </summary>
        public Requirement Requirement { get; }

        /// <summary>
        /// Gets the set of requirements that the requirement depended on that were missing.
        /// </summary>
        public IReadOnlyCollection<Requirement> MissingRequirements { get; }
    }
}
