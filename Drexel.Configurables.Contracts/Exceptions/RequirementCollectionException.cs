using System;

namespace Drexel.Configurables.Contracts.Exceptions
{
    public class RequirementCollectionException : Exception
    {
        public RequirementCollectionException(string message)
            : base(message)
        {
            // Nothing to do.
        }

        public RequirementCollectionException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Nothing to do.
        }
    }
}
