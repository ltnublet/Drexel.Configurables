using System.Collections;
using System.Collections.Generic;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables.Contracts
{
    public interface ISetValidator
    {
        void Validate(IEnumerable set);
    }

    public interface ISetValidator<T> : ISetValidator
    {
        /// <summary>
        /// Validates the supplied set against the specified <see cref="CollectionInfo"/>.
        /// </summary>
        /// <param name="set">
        /// The set to validate.
        /// </param>
        /// <exception cref="SetValidatorException">
        /// Thrown when an error occurs during validation. This means the set is not valid.
        /// </exception>
        void Validate(IEnumerable<T> set);
    }
}
