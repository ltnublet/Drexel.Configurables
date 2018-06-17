using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a configuration requirement.
    /// </summary>
    public interface IConfigurationRequirement
    {
        /// <summary>
        /// The set of <see cref="IConfigurationRequirement"/>s which must be supplied alongside this requirement.
        /// </summary>
        IEnumerable<IConfigurationRequirement> DependsOn { get; }

        /// <summary>
        /// The description of this requirement.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The set of <see cref="IConfigurationRequirement"/>s which must not be supplied alongside this requirement.
        /// </summary>
        IEnumerable<IConfigurationRequirement> ExclusiveWith { get; }

        /// <summary>
        /// <see langword="null"/> if this requirement expects a single instance of
        /// <see cref="ConfigurationRequirementType"/> <see cref="OfType"/>; else, the constraints of the required
        /// collection are described by the <see cref="CollectionInfo"/>.
        /// </summary>
        CollectionInfo CollectionInfo { get; }

        /// <summary>
        /// <see langword="true"/> if this requirement is optional; <see langword="false"/> if this requirement is
        /// required.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// The type of this requirement. This indicates what the expected <see cref="Type"/> of the input to
        /// <see cref="Validate(object, IConfiguration)"/> is.
        /// </summary>
        ConfigurationRequirementType OfType { get; }

        /// <summary>
        /// The name of this requirement.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Validates the supplied <see cref="object"/> for this requirement.
        /// </summary>
        /// <param name="instance">
        /// The <see cref="object"/> to perform validation upon.
        /// </param>
        /// <param name="dependentMappings">
        /// An <see cref="IConfiguration"/> containing <see cref="IMapping"/>s for all
        /// <see cref="IConfigurationRequirement"/>s in this requirement's
        /// <see cref="IConfigurationRequirement.DependsOn"/>.
        /// </param>
        /// <returns>
        /// <see langword="null"/> if the supplied <see cref="object"/> <paramref name="instance"/> passed validation;
        /// an <see cref="Exception"/> describing the validation failure otherwise.
        /// </returns>
        Exception Validate(object instance, IConfiguration dependentMappings = null);
    }
}
