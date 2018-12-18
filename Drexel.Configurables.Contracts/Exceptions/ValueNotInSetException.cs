using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The error thrown when a value is supplied that was not in the allowed set.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public sealed class ValueNotInSetException : SetValidatorWithValueException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueNotInSetException"/> class.
        /// </summary>
        /// <param name="value">
        /// The value associated with the error.
        /// </param>
        /// <param name="type">
        /// The type of the value associated with the error.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public ValueNotInSetException(object? value, Type type)
            : base("The supplied value is not present in the allowed set of values.", value, type)
        {
            // Nothing to do.
        }
    }
}
