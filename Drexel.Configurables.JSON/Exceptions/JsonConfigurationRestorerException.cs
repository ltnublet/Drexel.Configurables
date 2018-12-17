using System;

namespace Drexel.Configurables.Json.Exceptions
{
    public class JsonConfigurationRestorerException : Exception
    {
        public JsonConfigurationRestorerException(string message)
            : base(message)
        {
            // Nothing to do.
        }

        public JsonConfigurationRestorerException(string message, Exception innerException) 
            : base(message, innerException)
        {
            // Nothing to do.
        }
    }
}
