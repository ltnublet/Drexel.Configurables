using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The error thrown when a value is supplied that is of the wrong type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class ValueOfWrongTypeException : SetValidatorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueOfWrongTypeException"/> class.
        /// </summary>
        /// <param name="value">
        /// The value associated with the error.
        /// </param>
        /// <param name="expectedType">
        /// The expected type that the associated value did not satisfy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public ValueOfWrongTypeException(
            object? value,
            Type expectedType)
            : base("Supplied value is of the wrong type.")
        {
            this.Value = value;
            this.ExpectedType = expectedType ?? throw new ArgumentNullException(nameof(expectedType));
        }

        /// <summary>
        /// Gets the type that the associated value did not satisfy.
        /// </summary>
        public Type ExpectedType { get; }

        /// <summary>
        /// Gets the value associated with the error.
        /// </summary>
        public object? Value { get; }
    }
}
