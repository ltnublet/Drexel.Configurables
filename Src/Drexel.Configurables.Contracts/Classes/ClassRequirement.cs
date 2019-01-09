using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Drexel.Configurables.Contracts.Classes
{
    /// <summary>
    /// Represents a requirement for a <see langword="class"/> type.
    /// </summary>
    /// <typeparam name="T">
    /// The <see langword="class"/> type of the requirement.
    /// </typeparam>
    public sealed class ClassRequirement<T> : Requirement
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassRequirement{T}"/> class.
        /// </summary>
        /// <param name="id">
        /// The ID of this requirement.
        /// </param>
        /// <param name="type">
        /// The type of this requirement.
        /// </param>
        /// <param name="isOptional">
        /// <see langword="true"/> if this requirement is optional; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="collectionInfo">
        /// If this requirement is a collection, then a <see cref="CollectionInfo"/> describing constraints on values
        /// supplied against this requirement; otherwise, <see langword="null"/>.
        /// </param>
        /// <param name="restrictedToSet">
        /// If this requirement is restricted to a set of predefined values, then a collection of
        /// <see cref="ClassSetRestrictionInfo{T}"/>s describing constraints on the values this requirement is
        /// restricted to; otherwise, <see langword="null"/>.
        /// </param>
        /// <param name="dependsOn">
        /// If this requirement must be supplied alongside other requirements, then the set of
        /// <see cref="Requirement"/>s alongside which it must appear; otherwise, <see langword="null"/>.
        /// </param>
        /// <param name="exclusiveWith">
        /// If this requirement must not be supplied alongside other requirements, then the set of
        /// <see cref="Requirement"/>s alongside which it cannot appear; otherwise, <see langword="null"/>.
        /// </param>
        /// <param name="validationCallback">
        /// A callback that performs validation of possible bindings, if such logic exists.
        /// </param>
        /// <exception cref="Exceptions.DuplicateSetValueException">
        /// Thrown when <paramref name="restrictedToSet"/> contains duplicate values.
        /// </exception>
        public ClassRequirement(
            Guid id,
            ClassRequirementType<T> type,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<ClassSetRestrictionInfo<T>>? restrictedToSet = null,
            IReadOnlyCollection<Requirement>? dependsOn = null,
            IReadOnlyCollection<Requirement>? exclusiveWith = null,
            Func<object?, Configuration, Task>? validationCallback = null)
            : base(
                id,
                isOptional,
                type,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                validationCallback)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.RestrictedToSet = new ReadOnlyCollection<ClassSetRestrictionInfo<T>>(
                restrictedToSet == null
                    ? Array.Empty<ClassSetRestrictionInfo<T>>().ToList()
                    : restrictedToSet.ToList());
            this.SetValidator = new ClassSetValidator<T>(this.Type, this.RestrictedToSet, this.CollectionInfo);
        }

        /// <summary>
        /// Gets the set of values this requirement is restricted to, if such a restriction exists.
        /// </summary>
        public IReadOnlyCollection<ClassSetRestrictionInfo<T>>? RestrictedToSet { get; }

        /// <summary>
        /// Gets the set validator for this requirement.
        /// </summary>
        public new ClassSetValidator<T> SetValidator { get; }

        /// <summary>
        /// Gets the type of this requirement.
        /// </summary>
        public new ClassRequirementType<T> Type { get; }

        /// <summary>
        /// Gets the internal backing set validator.
        /// </summary>
        protected override SetValidator BackingSetValidator => this.SetValidator;
    }
}
