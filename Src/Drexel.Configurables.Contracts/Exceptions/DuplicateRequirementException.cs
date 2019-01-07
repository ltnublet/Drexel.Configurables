using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when duplicate requirements exist.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public sealed class DuplicateRequirementException : RequirementSetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateRequirementException"/> class.
        /// </summary>
        /// <param name="requirement">
        /// The requirement that had a duplicate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public DuplicateRequirementException(Requirement requirement)
            : base("Specified requirement set contains duplicate requirement.")
        {
            this.Duplicate = requirement ?? throw new ArgumentNullException(nameof(requirement));
        }

        /// <summary>
        /// Gets the requirement that had a duplicate.
        /// </summary>
        public Requirement Duplicate { get; }
    }
}
