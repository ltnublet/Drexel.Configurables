using System;
using System.Collections.Generic;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Persistables.Contracts;

namespace Drexel.Configurables.Persistables
{
    /// <summary>
    /// A simple <see cref="IPersistableConfigurationRequirement"/> implementation.
    /// </summary>
    public sealed class PersistableConfigurationRequirement : IPersistableConfigurationRequirement
    {
        private readonly IConfigurationRequirement requirement;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistableConfigurationRequirement"/> class using the supplied
        /// parameters.
        /// </summary>
        /// <param name="id">
        /// The unique ID that identifies this requirement.
        /// </param>
        /// <param name="version">
        /// The version of this requirement.
        /// </param>
        /// <param name="requirement">
        /// The nested requirement.
        /// </param>
        /// <param name="type">
        /// The type of this requirement.
        /// </param>
        public PersistableConfigurationRequirement(
            Guid id,
            Version version,
            IConfigurationRequirement requirement,
            PersistableConfigurationRequirementType type)
        {
            this.requirement = requirement ?? throw new ArgumentNullException(nameof(requirement));
            this.Id = id;
            this.Version = version ?? throw new ArgumentNullException(nameof(version));
            this.OfType = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// Gets the unique ID of this resource. This ID is invariant, and permanently identifies this resource.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the version of this resource.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Gets the set of <see cref="IConfigurationRequirement"/>s which must be supplied alongside this requirement.
        /// </summary>
        public IReadOnlyCollection<IConfigurationRequirement> DependsOn => this.requirement.DependsOn;

        /// <summary>
        /// Gets the description of this requirement.
        /// </summary>
        public string Description => this.requirement.Description;

        /// <summary>
        /// Gets the set of <see cref="IConfigurationRequirement"/>s which must not be supplied alongside this
        /// requirement.
        /// </summary>
        public IReadOnlyCollection<IConfigurationRequirement> ExclusiveWith => this.requirement.ExclusiveWith;

        /// <summary>
        /// Gets the collection constraints of this requirement, or <see langword="null"/> if none exist.
        /// </summary>
        public CollectionInfo CollectionInfo => this.requirement.CollectionInfo;

        /// <summary>
        /// Gets a value indicating whether this requirement is optional: <see langword="true"/> if this requirement is
        /// optional, or <see langword="false"/> if this requirement is required.
        /// </summary>
        public bool IsOptional => this.requirement.IsOptional;

        /// <summary>
        /// Gets the type of this requirement. This indicates what the expected <see cref="Type"/> of the input to
        /// <see cref="Validate(object, IConfiguration)"/> is.
        /// </summary>
        public PersistableConfigurationRequirementType OfType { get; }

        /// <summary>
        /// Gets the type of this requirement. This indicates what the expected <see cref="Type"/> of the input to
        /// <see cref="Validate(object, IConfiguration)"/> is.
        /// </summary>
         IConfigurationRequirementType IConfigurationRequirement.OfType => this.requirement.OfType;

        /// <summary>
        /// Gets the name of this requirement.
        /// </summary>
        public string Name => this.requirement.Name;

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
        public Exception Validate(object instance, IConfiguration dependentMappings = null) =>
            this.requirement.Validate(instance, dependentMappings);
    }
}
