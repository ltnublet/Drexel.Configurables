using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a set illegally contains duplicates.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class DuplicateSetValueException : SetValidatorWithValueException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateSetValueException"/> class.
        /// </summary>
        /// <param name="value">
        /// The value that has an illegal duplicate.
        /// </param>
        /// <param name="type">
        /// The type of the value associated with the error.
        /// </param>
        public DuplicateSetValueException(object? value, Type type)
            : base("Set contains illegal duplicate value.", value, type)
        {
            // Nothing to do.
        }
    }
}
