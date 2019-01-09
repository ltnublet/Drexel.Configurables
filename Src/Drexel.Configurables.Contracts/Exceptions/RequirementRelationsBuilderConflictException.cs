using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a <see cref="RequirementRelationsBuilder"/> is provided a
    /// <see cref="Requirement"/> that is in conflict with the current internal state of the builder.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class RequirementRelationsBuilderConflictException : RequirementRelationsBuilderException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementRelationsBuilderConflictException"/> class.
        /// </summary>
        /// <param name="specified">
        /// The requirement that was in conflict with an existing requirement.
        /// </param>
        /// <param name="existing">
        /// The existing requirement.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public RequirementRelationsBuilderConflictException(Requirement specified, Requirement existing)
            : base("The specified requirement is in conflict with an existing requirement.")
        {
            this.Specified = specified ?? throw new ArgumentNullException(nameof(specified));
            this.Existing = existing ?? throw new ArgumentNullException(nameof(existing));
        }

        /// <summary>
        /// Gets the requirement that was in conflict with an existing requirement.
        /// </summary>
        public Requirement Specified { get; }

        /// <summary>
        /// Gets the existing requirement.
        /// </summary>
        public Requirement Existing { get; }
    }
}
