using System;

namespace Drexel.Configurables.Json.Exceptions
{
    public class JsonStreamReaderException : Exception
    {
        public JsonStreamReaderException(string message)
            : base(message)
        {
            // Nothing to do.
        }

        public JsonStreamReaderException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Nothing to do.
        }
    }
}
