using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a <see cref="RequirementSet"/> contains a circular dependency.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class SetContainsCircularDependency : RequirementSetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetContainsCircularDependency"/> class.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property which has a cycle.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="propertyName"/> is <see langword="null"/>.
        /// </exception>
        public SetContainsCircularDependency(string propertyName)
            : base("Specified requirement set contains a circular dependency.")
        {
            this.PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        /// <summary>
        /// Gets the name of the property that contained the cycle.
        /// </summary>
        public string PropertyName { get; }
    }
}
