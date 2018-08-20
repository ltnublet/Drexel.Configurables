using System;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents the type of a configuration requirement.
    /// </summary>
    public interface IConfigurationRequirementType
    {
        /// <summary>
        /// Gets the <see cref="System.Type"/> of the this configuration requirement type.
        /// </summary>
        Type Type { get; }
    }
}
