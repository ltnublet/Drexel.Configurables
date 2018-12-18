using System;
using System.Collections.Generic;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// Represents a configuration requirement where the underlying type is a <see langword="struct"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of this requirement.
    /// </typeparam>
    public class StructRequirement<T> : IStructRequirement<T>
        where T : struct
    {
        private readonly Func<T, IConfiguration, Exception?>? validationCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructRequirement{T}"/> class.
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
        /// <param name="type">
        /// The <see cref="IStructRequirementType{T}"/> of this requirement.
        /// </param>
        /// <param name="isOptional">
        /// If <see langword="true"/>, this requirement is considered optional.
        /// </param>
        /// <param name="collectionInfo">
        /// If non-<see langword="null"/>, indicates that this requirement is a collection. The supplied
        /// <see cref="CollectionInfo"/> controls the collection constraints of this requirement.
        /// </param>
        /// <param name="restrictedToSet">
        /// If non-<see langword="null"/>, indicates that this requirement may only be satisfied by a value contained
        /// within the supplied set. If this requirement is a collection (as controlled by the
        /// <paramref name="collectionInfo"/> parameter), then all values within the collection must be contained by
        /// the supplied set.
        /// </param>
        /// <param name="dependsOn">
        /// If non-<see langword="null"/>, indicates that this requirement depends on the set of
        /// <see cref="IRequirement"/>s contained by the supplied set. This means that an <see cref="IConfiguration"/>
        /// containg this requirement must contain both this requirement, and all of the <see cref="IRequirement"/>s in
        /// the set.
        /// </param>
        /// <param name="exclusiveWith">
        /// If non-<see langword="null"/>, indicates that this requirement is exclusive with the set of
        /// <see cref="IRequirement"/>s contained by the supplied set. This means that an <see cref="IConfiguration"/>
        /// containing this requirement must not contain both this requirement, and any of the
        /// <see cref="IRequirement"/>s in the set.
        /// </param>
        /// <param name="validationCallback">
        /// A callback which performs additional validation for values supplied against this requirement. The callback
        /// should return an <see cref="Exception"/> if the value is not valid, or <see langword="null"/> if the value
        /// is valid.
        /// </param>
        public StructRequirement(
            Guid id,
            string name,
            string description,
            IStructRequirementType<T> type,
            bool isOptional,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<SetRestrictionInfo<T>>? restrictedToSet = null,
            IReadOnlyCollection<IRequirement>? dependsOn = null,
            IReadOnlyCollection<IRequirement>? exclusiveWith = null,
            Func<T, IConfiguration, Exception?>? validationCallback = null)
        {
            this.Id = id;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
            this.Type = type;
            this.IsOptional = isOptional;
            this.CollectionInfo = collectionInfo;
            this.RestrictedToSet = restrictedToSet;
            this.DependsOn = dependsOn ?? Array.Empty<IRequirement>();
            this.ExclusiveWith = exclusiveWith ?? Array.Empty<IRequirement>();
            this.validationCallback = validationCallback;
        }

        /// <summary>
        /// Gets the description of this requirement.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the set of <see cref="IRequirement"/>s this requirement depends on.
        /// </summary>
        public IReadOnlyCollection<IRequirement> DependsOn { get; }

        /// <summary>
        /// Gets the collection constraints of this requirement, if any exist.
        /// </summary>
        public CollectionInfo? CollectionInfo { get; }

        /// <summary>
        /// Gets the set of <see cref="IRequirement"/>s this requirement is exclusive with.
        /// </summary>
        public IReadOnlyCollection<IRequirement> ExclusiveWith { get; }

        /// <summary>
        /// Gets the ID of this requirement.
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
        /// Gets the set of <see cref="SetRestrictionInfo{T}"/>s describing the set of values this requirement is
        /// restricted to, if such restrictions exist; otherwise, <see langword="null"/>.
        /// </summary>
        public IReadOnlyCollection<SetRestrictionInfo<T>>? RestrictedToSet { get; }

        /// <summary>
        /// Gets the type of this requirement.
        /// </summary>
        public IStructRequirementType<T> Type { get; }

        /// <summary>
        /// Gets the type of this requirement.
        /// </summary>
        IRequirementType IRequirement.Type => this.Type;

        /// <summary>
        /// Creates the set validator for this requirement.
        /// </summary>
        /// <returns>
        /// The set validator for this requirement.
        /// </returns>
        public ISetValidator<T> CreateSetValidator() =>
            new SetValidator<T>(this.RestrictedToSet, this.CollectionInfo);

        /// <summary>
        /// Creates the set validator for this requirement.
        /// </summary>
        /// <returns>
        /// The set validator for this requirement.
        /// </returns>
        ISetValidator IRequirement.CreateSetValidator() => this.CreateSetValidator();

        /// <summary>
        /// Validates the supplied value against this requirement.
        /// </summary>
        /// <param name="value">
        /// The value to validate.
        /// </param>
        /// <param name="dependencies">
        /// An <see cref="IConfiguration"/> containing the dependencies of this requirement, and validated values
        /// mapped to them.
        /// </param>
        /// <returns>
        /// If the value passes validation, <see langword="null"/>; otherwise, an <see cref="Exception"/> describing
        /// the reason validation failed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dependencies"/> is <see langword="null"/>.
        /// </exception>
        public Exception? Validate(T value, IConfiguration dependencies)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            return this.validationCallback?.Invoke(value, dependencies);
        }

        /// <summary>
        /// Validates the supplied value against this requirement.
        /// </summary>
        /// <param name="value">
        /// The value to validate.
        /// </param>
        /// <param name="dependencies">
        /// An <see cref="IConfiguration"/> containing the dependencies of this requirement, and validated values
        /// mapped to them.
        /// </param>
        /// <returns>
        /// If the value passes validation, <see langword="null"/>; otherwise, an <see cref="Exception"/> describing
        /// the reason validation failed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dependencies"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown when the supplied <paramref name="value"/> is not of type <typeparamref name="T"/>.
        /// </exception>
        public Exception? Validate(object? value, IConfiguration dependencies)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            if (value == null)
            {
                return new ArgumentNullException(nameof(value));
            }
            else if (value is T asT)
            {
                return this.Validate(asT, dependencies);
            }

            return new InvalidCastException();
        }
    }
}
