using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an error occurs while using an <see cref="ISetValidator"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class SetValidatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetValidatorException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public SetValidatorException(string message)
            : this(message, null)
        {
            // Nothing to do.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetValidatorException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception
        /// is specified.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public SetValidatorException(string message, Exception? innerException)
            : base(
                  message ?? throw new ArgumentNullException(nameof(message)),
                  innerException)
        {
            // Nothing to do.
        }
    }
}
