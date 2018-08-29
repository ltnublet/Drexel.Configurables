using System;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Represents a resource which can be stored and, at a later unspecified time, restored.
    /// </summary>
    public interface IPersistable
    {
        /// <summary>
        /// Gets the unique ID of this resource. This ID is invariant, and permanently identifies this resource.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the version of this resource.
        /// </summary>
        Version Version { get; }
    }
}
