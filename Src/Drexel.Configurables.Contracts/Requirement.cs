using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Drexel.Configurables.Contracts
{
    public abstract class Requirement
    {
        private readonly Func<object?, Configuration, Task>? validationCallback;

        private protected Requirement(
            Guid id,
            bool isOptional,
            RequirementType type,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<Requirement>? dependsOn = null,
            IReadOnlyCollection<Requirement>? exclusiveWith = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            this.Id = id;
            this.IsOptional = isOptional;
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.CollectionInfo = collectionInfo;
            this.DependsOn = dependsOn ?? Array.Empty<Requirement>();
            this.ExclusiveWith = dependsOn ?? Array.Empty<Requirement>();
            this.validationCallback = validationCallback;
        }

        public Guid Id { get; }

        public bool IsOptional { get; }

        public RequirementType Type { get; }

        public CollectionInfo? CollectionInfo { get; }

        public IReadOnlyCollection<Requirement> DependsOn { get; }

        public IReadOnlyCollection<Requirement> ExclusiveWith { get; }

        public SetValidator SetValidator => this.BackingSetValidator;

        protected abstract SetValidator BackingSetValidator { get; }

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
