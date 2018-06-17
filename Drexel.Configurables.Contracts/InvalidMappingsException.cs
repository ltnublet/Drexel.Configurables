using System;
using System.Runtime.Serialization;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// The exception that is thrown when a set of mappings is invalid.
    /// </summary>
    [Serializable]
    public sealed class InvalidMappingsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMappingsException"/> class.
        /// </summary>
        public InvalidMappingsException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMappingsException"/> class with a specified error
        /// <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public InvalidMappingsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMappingsException"/> class with a specified error
        /// <paramref name="message"/> and a reference to the inner <see cref="Exception"/>
        /// <paramref name="innerException"/> that is the cause of this <see cref="InvalidMappingsException"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="innerException">
        /// The <see cref="Exception"/> that is the cause of the current <see cref="InvalidMappingsException"/>, or
        /// <see langword="null"/> if no inner <see cref="Exception"/> is specified.
        /// </param>
        public InvalidMappingsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private InvalidMappingsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
