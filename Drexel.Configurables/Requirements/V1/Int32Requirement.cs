using System;
using System.Collections.Generic;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Requirements.V1
{
    /// <summary>
    /// Represents an <see cref="IRequirement{T}"/> of type <see cref="int"/>.
    /// </summary>
    public sealed class Int32Requirement : IRequirement<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolRequirement"/> class.
        /// </summary>
        /// <param name="id">
        /// The ID of this requirement. An ID must be unique within a given <see cref="IConfiguration"/>.
        /// </param>
        /// <param name="name">
        /// The name of this requirement.
        /// </param>
        /// <param name="description">
        /// The description of this requirement.
        /// </param>
        /// <param name="isOptional">
        /// If <see langword="true"/>, this requirement is considered optional.
        /// </param>
        /// <param name="enumerableInfo">
        /// If non-<see langword="null"/>, indicates that this requirement is a collection. The supplied
        /// <see cref="EnumerableInfo"/> controls the collection constraints of this requirement.
        /// </param>
        /// <param name="restrictedToSet">
        /// If non-<see langword="null"/>, indicates that this requirement may only be satisfied by a value contained
        /// within the supplied set. If this requirement is a collection (as controlled by the
        /// <paramref name="enumerableInfo"/> parameter), then all values within the collection must be contained by
        /// the supplied set.
        /// </param>
        /// <param name="dependsOn">
        /// If non-<see langword="null"/>, indicates that this requirement depends on the set of
        /// <see cref="IRequirement"/>s contained by the supplied set. This means that an <see cref="IConfiguration"/>
        /// containg this requirement must contain both this requirement, and all of the <see cref="IRequirement"/>s in
        /// the set.
        /// </param>
        /// <param name="exclusivewith">
        /// If non-<see langword="null"/>, indicates that this requirement is exclusive with the set of
        /// <see cref="IRequirement"/>s contained by the supplied set. This means that an <see cref="IConfiguration"/>
        /// containing this requirement must not contain both this requirement, and any of the
        /// <see cref="IRequirement"/>s in the set.
        /// </param>
        public Int32Requirement(
            Guid id,
            string name,
            string description,
            bool isOptional,
            EnumerableInfo? enumerableInfo = null,
            IReadOnlyCollection<SetRestrictionInfo<int>> restrictedToSet = null,
            IReadOnlyCollection<IRequirement> dependsOn = null,
            IReadOnlyCollection<IRequirement> exclusivewith = null)
        {
            this.Id = id;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
            this.IsOptional = isOptional;
            this.EnumerableInfo = enumerableInfo;
            this.RestrictedToSet = restrictedToSet;
            this.DependsOn = dependsOn;
            this.ExclusiveWith = exclusivewith;
        }

        /// <summary>
        /// Gets the description of this requirement.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the set of <see cref="IRequirement"/>s this requirement depends on. This means that an
        /// <see cref="IConfiguration"/> containg this requirement must contain both this requirement, and all of the
        /// <see cref="IRequirement"/>s in the set.
        /// </summary>
        public IReadOnlyCollection<IRequirement> DependsOn { get; }

        /// <summary>
        /// Gets the collection constraints of this requirement.
        /// </summary>
        public EnumerableInfo? EnumerableInfo { get; }

        /// <summary>
        /// Gets the set of <see cref="IRequirement"/>s this requirement is exclusive with. This means that an
        /// <see cref="IConfiguration"/> containing this requirement must not contain both this requirement, and any of
        /// the <see cref="IRequirement"/>s in the set.
        /// </summary>
        public IReadOnlyCollection<IRequirement> ExclusiveWith { get; }

        /// <summary>
        /// The ID of this requirement. An ID must be unique within a given <see cref="IConfiguration"/>.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets a value indicating whether this requirement is optional.
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        /// Gets the name of this requirement.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the set of values this requirement is restricted to.
        /// </summary>
        public IReadOnlyCollection<SetRestrictionInfo<int>> RestrictedToSet { get; }

        /// <summary>
        /// Gets the type of this requirement.
        /// </summary>
        public IRequirementType<int> Type => RequirementTypes.V1.Int32;

        /// <summary>
        /// Gets the type of this requirement.
        /// </summary>
        IRequirementType IRequirement.Type => this.Type;
    }
}
