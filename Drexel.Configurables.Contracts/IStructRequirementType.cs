using System;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a strongly-typed type of a requirement where the underlying type is a <see langword="struct"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying type of this requirement type.
    /// </typeparam>
    public interface IStructRequirementType<T> : IRequirementType
        where T : struct
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
        /// <exception cref="ArgumentNullException">
        /// Thrown when the supplied value is illegally <see langword="null"/>. Since a <see langword="struct"/> cannot
        /// have a value of <see langword="null"/>, this will happen if <paramref name="value"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown when the supplied <paramref name="value"/> cannot be cast.
        /// </exception>
        T Cast(object value);

        /// <summary>
        /// Persists the supplied value, if possible.
        /// </summary>
        /// <param name="value">
        /// The value to persist.
        /// </param>
        /// <returns>
        /// A <see langword="string"/> representing the value, if possible.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the supplied value is illegally <see langword="null"/>. Since a <see langword="struct"/> cannot
        /// have a value of <see langword="null"/>, this will happen if <paramref name="value"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when this type does not support persisting.
        /// </exception>
        string Persist(T value);

        /// <summary>
        /// Restores the supplied <see langword="string"/> representation of a value, if possible.
        /// </summary>
        /// <param name="value">
        /// The <see langword="string"/> representation of a value.
        /// </param>
        /// <returns>
        /// An <see cref="object"/> based on the supplied <paramref name="value"/>, if possible.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the supplied value is illegally <see langword="null"/>. Since a <see langword="struct"/> cannot
        /// have a value of <see langword="null"/>, this will happen if <paramref name="value"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when this type does not support persisting.
        /// </exception>
        new T Restore(string value);
    }
}
