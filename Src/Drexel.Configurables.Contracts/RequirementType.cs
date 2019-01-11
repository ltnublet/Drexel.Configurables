using System;
using System.Collections;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents the type of a requirement.
    /// </summary>
    public abstract class RequirementType
    {
        protected RequirementType(Guid id, Type type)
        {
            this.Id = id;
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// The ID of the requirement type.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The underlying <see cref="Type"/> of the requirement type.
        /// </summary>
        public Type Type { get; }

        public abstract bool TryCast(object? value, out object? result);

        public abstract bool TryCast(object? value, out IEnumerable? result);
    }
}
