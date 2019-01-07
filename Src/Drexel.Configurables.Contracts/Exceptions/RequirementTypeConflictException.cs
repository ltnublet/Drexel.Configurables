using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a <see cref="RequirementSet"/> contains conflicting
    /// <see cref="RequirementType"/>s.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public sealed class RequirementTypeConflictException : RequirementSetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementTypeConflictException"/> class.
        /// </summary>
        /// <param name="first">
        /// The first requirement whose was in conflict.
        /// </param>
        /// <param name="second">
        /// The second requirement whose was in conflict.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when either <paramref name="first"/> or <paramref name="second"/> is <see langword="null"/>.
        /// </exception>
        public RequirementTypeConflictException(Requirement first, Requirement second)
            : base("Specified requirement set contains conflicting requirement types.")
        {
            this.First = first ?? throw new ArgumentNullException(nameof(first));
            this.Second = second ?? throw new ArgumentNullException(nameof(second));
        }

        /// <summary>
        /// The first requirement whose type was in conflict.
        /// </summary>
        public Requirement First { get; }

        /// <summary>
        /// The second requirement whose type was in conflict.
        /// </summary>
        public Requirement Second { get; }
    }
}
