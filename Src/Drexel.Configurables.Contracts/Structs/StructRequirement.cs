using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Drexel.Configurables.Contracts.Structs
{
    /// <summary>
    /// Represents a requirement for a <see langword="struct"/> type.
    /// </summary>
    /// <typeparam name="T">
    /// The <see langword="struct"/> type of the requirement.
    /// </typeparam>
    public sealed class StructRequirement<T> : Requirement
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructRequirement{T}"/> class.
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
        /// <see cref="StructSetRestrictionInfo{T}"/>s describing constraints on the values this requirement is
        /// restricted to; otherwise, <see langword="null"/>.
        /// </param>
        /// <param name="validationCallback">
        /// A callback that performs validation of possible bindings, if such logic exists.
        /// </param>
        /// <param name="defaultValue">
        /// The default value of this requirement, if one exists. Note that if this is optional or collection
        /// requirement, then this value is only used by external callers.
        /// </param>
        /// <param name="relations">
        /// If this requirement has defined relations with other requirements (such as this requirement depending on or
        /// being exclusive with another), then a <see cref="RequirementRelations"/> describing these relations;
        /// otherwise, <see langword="null"/>.
        /// </param>
        /// <exception cref="Exceptions.DuplicateSetValueException">
        /// Thrown when <paramref name="restrictedToSet"/> contains duplicate values.
        /// </exception>
        public StructRequirement(
            Guid id,
            StructRequirementType<T> type,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<T>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<T>>? restrictedToSet = null)
            : base(
                id,
                type,
                isOptional,
                collectionInfo,
                relations,
                validationCallback)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.RestrictedToSet = new ReadOnlyCollection<StructSetRestrictionInfo<T>>(
                 restrictedToSet == null
                     ? Array.Empty<StructSetRestrictionInfo<T>>().ToList()
                     : restrictedToSet.ToList());
            this.SetValidator = new StructSetValidator<T>(this.Type, this.RestrictedToSet, this.CollectionInfo);
            this.DefaultValue = defaultValue ?? new StructDefaultValue<T>();
        }

        /// <summary>
        /// Gets the set of values this requirement is restricted to, if such a restriction exists.
        /// </summary>
        public IReadOnlyCollection<StructSetRestrictionInfo<T>>? RestrictedToSet { get; }

        /// <summary>
        /// Gets the set validator for this requirement.
        /// </summary>
        public new StructSetValidator<T> SetValidator { get; private set; }

        /// <summary>
        /// Gets the type of this requirement.
        /// </summary>
        public new StructRequirementType<T> Type { get; }

        /// <summary>
        /// Gets the default value of this requirement.
        /// </summary>
        public new StructDefaultValue<T> DefaultValue { get; }

        /// <summary>
        /// Gets the internal backing set validator.
        /// </summary>
        protected override SetValidator BackingSetValidator => this.SetValidator;

        /// <summary>
        /// Gets the internal backing default value.
        /// </summary>
        protected override DefaultValue BackingDefaultValue => throw new NotImplementedException();
    }
}
