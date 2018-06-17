using System;
using System.Runtime.Serialization;

namespace Drexel.Configurables
{
    /// <summary>
    /// The exception that is thrown when a set of requirements is invalid.
    /// </summary>
    [Serializable]
    public sealed class InvalidRequirementsException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequirementsException"/> class.
        /// </summary>
        public InvalidRequirementsException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequirementsException"/> class with a specified error
        /// <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public InvalidRequirementsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequirementsException"/> class with a specified error
        /// <paramref name="message"/> and the name of the parameter that caused this exception.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current <see cref="InvalidRequirementsException"/>.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "param",
            Justification = "Following pre-existing naming convention.")]
        public InvalidRequirementsException(string message, string paramName)
            : base(message, paramName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequirementsException"/> class with a specified error
        /// <paramref name="message"/> and a reference to the inner <see cref="Exception"/>
        /// <paramref name="innerException"/> that is the cause of this <see cref="InvalidRequirementsException"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="innerException">
        /// The <see cref="Exception"/> that is the cause of the current <see cref="InvalidRequirementsException"/>, or
        /// <see langword="null"/> if no inner <see cref="Exception"/> is specified.
        /// </param>
        public InvalidRequirementsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequirementsException"/> class with a specified error
        /// <paramref name="message"/>, the parameter name <paramref name="paramName"/>, and a reference to the inner
        /// <see cref="Exception"/> <paramref name="innerException"/> that is the cause of this
        /// <see cref="InvalidRequirementsException"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current <see cref="InvalidRequirementsException"/>.
        /// </param>
        /// <param name="innerException">
        /// The <see cref="Exception"/> that is the cause of the current <see cref="InvalidRequirementsException"/>, or
        /// <see langword="null"/> if no inner <see cref="Exception"/> is specified.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "param",
            Justification = "Following pre-existing naming convention.")]
        public InvalidRequirementsException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException)
        {
        }

        private InvalidRequirementsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
