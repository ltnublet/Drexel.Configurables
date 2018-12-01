using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    public interface IRequirement
    {
        string Description { get; }

        IReadOnlyCollection<IRequirement> DependsOn { get; }

        EnumerableInfo? EnumerableInfo { get; }

        IReadOnlyCollection<IRequirement> ExclusiveWith { get; }

        Guid Id { get; }

        bool IsOptional { get; }

        string Name { get; }

        IRequirementType Type { get; }
    }

    public interface IRequirement<T> : IRequirement
    {
        IReadOnlyCollection<SetRestrictionInfo<T>> RestrictedToSet { get; }

        new IRequirementType<T> Type { get; }
    }
}
