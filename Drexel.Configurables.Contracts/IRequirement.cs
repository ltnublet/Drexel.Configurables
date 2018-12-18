using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a requirement.
    /// </summary>
    public interface IRequirement
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the set of <see cref="IRequirement"/>s on which this <see cref="IRequirement"/> depends.
        /// </summary>
        IReadOnlyCollection<IRequirement> DependsOn { get; }

        /// <summary>
        /// Gets the <see cref="CollectionInfo"/> of this <see cref="IRequirement"/>, if one exists.
        /// </summary>
        CollectionInfo? CollectionInfo { get; }

        /// <summary>
        /// Gets the set of <see cref="IRequirement"/>s which this <see cref="IRequirement"/> is exclusive with.
        /// </summary>
        IReadOnlyCollection<IRequirement> ExclusiveWith { get; }

        /// <summary>
        /// Gets the unique identifier of this requirement.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IRequirement"/> is optional.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// Gets the name of this requirement.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of this requirement.
        /// </summary>
        IRequirementType Type { get; }

        /// <summary>
        /// Creates an <see cref="ISetValidator"/> for this <see cref="IRequirement"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="ISetValidator"/> for this <see cref="IRequirement"/>.
        /// </returns>
        ISetValidator CreateSetValidator();

        /// <summary>
        /// Validates the specified value against this requirement.
        /// </summary>
        /// <param name="value">
        /// The value to validate.
        /// </param>
        /// <param name="dependencies">
        /// The dependencies of this requirement.
        /// </param>
        /// <returns>
        /// If the <paramref name="value"/> was not valid, an <see cref="Exception"/> describing the validation
        /// failure; otherwise, <see langword="null"/>.
        /// </returns>
        Exception? Validate(object value, IConfiguration dependencies);
    }

    /// <summary>
    /// Represents a strongly-typed requirement.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the requirement.
    /// </typeparam>
    public interface IRequirement<T> : IRequirement
    {
        /// <summary>
        /// Gets the set of <see cref="SetRestrictionInfo{T}"/>s which this requirement is restricted to.
        /// </summary>
        IReadOnlyCollection<SetRestrictionInfo<T>>? RestrictedToSet { get; }

        /// <summary>
        /// Gets the type of this requirement.
        /// </summary>
        new IRequirementType<T> Type { get; }

        /// <summary>
        /// Creates an <see cref="ISetValidator{T}"/> for this <see cref="IRequirement"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="ISetValidator{T}"/> for this <see cref="IRequirement"/>.
        /// </returns>
        new ISetValidator<T> CreateSetValidator();

        /// <summary>
        /// Validates the specified value against this requirement.
        /// </summary>
        /// <param name="value">
        /// The value to validate.
        /// </param>
        /// <param name="dependencies">
        /// The dependencies of this requirement.
        /// </param>
        /// <returns>
        /// If the <paramref name="value"/> was not valid, an <see cref="Exception"/> describing the validation
        /// failure; otherwise, <see langword="null"/>.
        /// </returns>
        Exception? Validate(T value, IConfiguration dependencies);
    }
}
