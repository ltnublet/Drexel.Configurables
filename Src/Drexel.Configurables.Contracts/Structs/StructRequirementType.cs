using System;
using System.Collections;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Structs
{
    /// <summary>
    /// Represents a <see langword="struct"/> requirement type.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying <see langword="struct"/> type.
    /// </typeparam>
    public sealed class StructRequirementType<T> : RequirementType
        where T : struct
    {
        public delegate bool TryCastValue(object? value, out T result);

        public delegate bool TryCastCollection(object? value, out IEnumerable<T>? result);

        private readonly TryCastValue tryCastValue;
        private readonly TryCastCollection tryCastCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructRequirementType{T}"/> class.
        /// </summary>
        /// <param name="id">
        /// The ID of the requirement type.
        /// </param>
        /// <param name="tryCastValue">
        /// A delegate that casts individual <see langword="object"/>s to <typeparamref name="T"/>s.
        /// </param>
        /// <param name="tryCastCollection">
        /// A delegate that casts collections of <see langword="object"/>s to collections of <typeparamref name="T"/>s.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally null.
        /// </exception>
        public StructRequirementType(
            Guid id,
            TryCastValue tryCastValue,
            TryCastCollection tryCastCollection)
            : base(id, typeof(T))
        {
            this.tryCastValue = tryCastValue ?? throw new ArgumentNullException(nameof(tryCastValue));
            this.tryCastCollection = tryCastCollection ?? throw new ArgumentNullException(nameof(tryCastCollection));
        }

        public bool TryCast(object? value, out T result) => this.tryCastValue(value, out result);

        public override bool TryCast(object? value, out object result)
        {
            bool status = this.TryCast(value, out T buffer);
            result = buffer;
            return status;
        }

        public bool TryCast(object? value, out IEnumerable<T>? result) => this.tryCastCollection(value, out result);

        public override bool TryCast(object? value, out IEnumerable? result)
        {
            bool status = this.TryCast(value, out IEnumerable<T>? buffer);
            result = buffer;
            return status;
        }
    }
}
