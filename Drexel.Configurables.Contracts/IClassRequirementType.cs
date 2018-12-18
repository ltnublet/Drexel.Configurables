using System;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a strongly-typed type of a requirement where the underlying type is a <see langword="class"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying type of this requirement type.
    /// </typeparam>
    public interface IClassRequirementType<T> : IRequirementType
        where T : class
    {
        /// <summary>
        /// Casts an <see cref="object"/> to a <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="object"/> to cast.
        /// </param>
        /// <returns>
        /// A <typeparamref name="T"/> based on the supplied <paramref name="value"/>.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Thrown when the supplied <paramref name="value"/> cannot be cast.
        /// </exception>
        T? Cast(object? value);

        /// <summary>
        /// Persists the supplied value, if possible.
        /// </summary>
        /// <param name="value">
        /// The value to persist.
        /// </param>
        /// <returns>
        /// A <see langword="string"/> representing the value, if possible.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when this type does not support persisting.
        /// </exception>
        string? Persist(T? value);

        /// <summary>
        /// Restores the supplied <see langword="string"/> representation of a value, if possible.
        /// </summary>
        /// <param name="value">
        /// The <see langword="string"/> representation of a value.
        /// </param>
        /// <returns>
        /// An <see cref="object"/> based on the supplied <paramref name="value"/>, if possible.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when this type does not support persisting.
        /// </exception>
        new T? Restore(string? value);
    }
}
