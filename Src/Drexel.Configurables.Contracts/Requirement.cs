using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Drexel.Configurables.Contracts
{
    public abstract class Requirement
    {
        private readonly Func<object?, Configuration, Task>? validationCallback;

        private protected Requirement(
            Guid id,
            RequirementType type,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            this.Id = id;
            this.IsOptional = isOptional;
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.CollectionInfo = collectionInfo;
            this.DependsOn = relations?.DependsOn
                ?? new ReadOnlyCollection<Requirement>(Array.Empty<Requirement>().ToList());
            this.ExclusiveWith = relations?.ExclusiveWith
                ?? new ReadOnlyCollection<Requirement>(Array.Empty<Requirement>().ToList());
            this.validationCallback = validationCallback;

            if (this.ExclusiveWith.Any(x => !x.IsOptional))
            {
                throw new ArgumentException(
                    "An argument cannot be exclusive with a required argument.",
                    nameof(relations));
            }
        }

        public Guid Id { get; }

        public bool IsOptional { get; }

        public RequirementType Type { get; }

        public CollectionInfo? CollectionInfo { get; }

        public IReadOnlyCollection<Requirement> DependsOn { get; }

        public IReadOnlyCollection<Requirement> ExclusiveWith { get; }

        public SetValidator SetValidator => this.BackingSetValidator;

        public DefaultValue DefaultValue => this.BackingDefaultValue;

        protected abstract SetValidator BackingSetValidator { get; }

        protected abstract DefaultValue BackingDefaultValue { get; }

        public async Task Validate(object? value, Configuration dependencies)
        {
            if (this.validationCallback != null)
            {
                if (dependencies.Requirements.IsSupersetOf(this.DependsOn))
                {
                    await this.validationCallback.Invoke(value, dependencies).ConfigureAwait(false);
                }
                else
                {
                    throw new ArgumentException(
                        "Specified configuration does not contain all dependencies of this requirement.");
                }
            }
        }
    }
}
