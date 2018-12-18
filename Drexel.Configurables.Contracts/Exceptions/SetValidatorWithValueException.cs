using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The error that is thrown when an error occurs while using an <see cref="ISetValidator"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class SetValidatorWithValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetValidatorWithValueException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="value">
        /// The value associated with the error.
        /// </param>
        /// <param name="type">
        /// The type of the value associated with the error.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public SetValidatorWithValueException(string message, object? value, Type type)
            : base(message ?? throw new ArgumentNullException(nameof(message)))
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Value = value;
        }

        /// <summary>
        /// Gets the type of the value associated with the error.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the value associated with the error.
        /// </summary>
        public object? Value { get; }
    }
}
