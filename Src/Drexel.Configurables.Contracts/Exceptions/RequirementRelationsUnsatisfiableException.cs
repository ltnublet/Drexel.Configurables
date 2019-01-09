using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a <see cref="RequirementRelationsBuilder"/> is provided a
    /// <see cref="Requirement"/> that would result in an unsatisfiable <see cref="RequirementRelations"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class RequirementRelationsUnsatisfiableException : RequirementRelationsBuilderException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementRelationsUnsatisfiableException"/> class.
        /// </summary>
        /// <param name="cannotAdd">
        /// The requirement that could not be added because doing so would result in an unsatisfiable
        /// <see cref="RequirementRelations"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public RequirementRelationsUnsatisfiableException(Requirement cannotAdd)
            : base("Adding the specified requirement would result in an unsatisfiable set of relations.")
        {
            this.CannotAdd = cannotAdd ?? throw new ArgumentNullException(nameof(cannotAdd));
        }

        /// <summary>
        /// Gets the requirement that could not be added.
        /// </summary>
        public Requirement CannotAdd { get; }
    }
}
