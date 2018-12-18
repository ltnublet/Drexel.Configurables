using System.Collections;
using System.Collections.Generic;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a set validator.
    /// </summary>
    public interface ISetValidator
    {
        /// <summary>
        /// Validates the supplied set.
        /// </summary>
        /// <param name="set">
        /// The set to validate.
        /// </param>
        /// <exception cref="SetValidatorException">
        /// Thrown when an error occurs during validation. This means the set is not valid.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming",
            "CA1716:Identifiers should not match keywords",
            Justification = "It is not a keyword.")]
        void Validate(IEnumerable set);
    }

    /// <summary>
    /// Represents a strongly-typed set validator.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the set to validate.
    /// </typeparam>
    public interface ISetValidator<T> : ISetValidator
    {
        /// <summary>
        /// Validates the supplied set.
        /// </summary>
        /// <param name="set">
        /// The set to validate.
        /// </param>
        /// <exception cref="SetValidatorException">
        /// Thrown when an error occurs during validation. This means the set is not valid.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming",
            "CA1716:Identifiers should not match keywords",
            Justification = "It is not a keyword.")]
        void Validate(IEnumerable<T> set);
    }
}
