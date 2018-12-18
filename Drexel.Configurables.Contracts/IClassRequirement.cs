using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a strongly-typed requirement where the underlying type is a <see langword="class"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the requirement.
    /// </typeparam>
    public interface IClassRequirement<T> : IRequirement
        where T : class
    {
        /// <summary>
        /// Gets the set of <see cref="SetRestrictionInfo{T}"/>s which this requirement is restricted to.
        /// </summary>
        IReadOnlyCollection<SetRestrictionInfo<T>>? RestrictedToSet { get; }

        /// <summary>
        /// Gets the type of this requirement.
        /// </summary>
        new IClassRequirementType<T> Type { get; }

        /// <summary>
        /// Creates an <see cref="ISetValidator{T}"/> for this <see cref="IClassRequirement{T}"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="ISetValidator{T}"/> for this <see cref="IClassRequirement{T}"/>.
        /// </returns>
        new ISetValidator<T> CreateSetValidator();

        /// <summary>
        /// Validates the specified value against this <see cref="IClassRequirement{T}"/>.
        /// </summary>
        /// <param name="value">
        /// The value to validate.
        /// </param>
        /// <param name="dependencies">
        /// The dependencies of this <see cref="IClassRequirement{T}"/>.
        /// </param>
        /// <returns>
        /// If the <paramref name="value"/> was not valid, an <see cref="Exception"/> describing the validation
        /// failure; otherwise, <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dependencies"/> is <see langword="null"/>.
        /// </exception>
        Exception? Validate(T? value, IConfiguration dependencies);
    }
}
