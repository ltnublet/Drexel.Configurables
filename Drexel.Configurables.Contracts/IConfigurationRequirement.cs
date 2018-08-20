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
        /// Gets the set of <see cref="IConfigurationRequirement"/>s which must be supplied alongside this requirement.
        /// </summary>
        IReadOnlyCollection<IConfigurationRequirement> DependsOn { get; }

        /// <summary>
        /// Gets the description of this requirement.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the set of <see cref="IConfigurationRequirement"/>s which must not be supplied alongside this
        /// requirement.
        /// </summary>
        IReadOnlyCollection<IConfigurationRequirement> ExclusiveWith { get; }

        /// <summary>
        /// Gets the collection constraints of this requirement, or <see langword="null"/> if none exist.
        /// </summary>
        CollectionInfo CollectionInfo { get; }

        /// <summary>
        /// Gets a value indicating whether this requirement is optional: <see langword="true"/> if this requirement is
        /// optional, or <see langword="false"/> if this requirement is required.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// Gets the type of this requirement. This indicates what the expected <see cref="Type"/> of the input to
        /// <see cref="Validate(object, IConfiguration)"/> is.
        /// </summary>
        IConfigurationRequirementType OfType { get; }

        /// <summary>
        /// Gets the name of this requirement.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Validates the supplied <see cref="object"/> for this requirement.
        /// </summary>
        /// <param name="instance">
        /// The <see cref="object"/> to perform validation upon.
        /// </param>
        /// <param name="dependentMappings">
        /// An <see cref="IConfiguration"/> containing <see cref="IMapping{IConfigurationRequirement}"/>s for all
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
