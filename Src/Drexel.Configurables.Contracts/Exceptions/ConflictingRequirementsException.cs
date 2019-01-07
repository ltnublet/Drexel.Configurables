using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a requirement is in conflict with other requirements.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public sealed class ConflictingRequirementsException : RequirementSetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictingRequirementsException"/> class.
        /// </summary>
        /// <param name="requirement">
        /// The requirement that had a conflict.
        /// </param>
        /// <param name="conflictingRequirements">
        /// The set of requirements that were in conflict with the requirement.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public ConflictingRequirementsException(
            Requirement requirement,
            IReadOnlyCollection<Requirement> conflictingRequirements)
            : base("Specified requirement set contains a conflict for the specified requirement.")
        {
            this.Requirement = requirement
                ?? throw new ArgumentNullException(nameof(requirement));
            this.ConflictingRequirements = conflictingRequirements
                ?? throw new ArgumentNullException(nameof(conflictingRequirements));
        }

        /// <summary>
        /// Gets the requirement that had a conflict.
        /// </summary>
        public Requirement Requirement { get; }

        /// <summary>
        /// Gets the set of requirements that were in conflict with the requirement.
        /// </summary>
        public IReadOnlyCollection<Requirement> ConflictingRequirements { get; }
    }
}
