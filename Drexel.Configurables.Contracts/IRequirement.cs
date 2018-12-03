using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    public interface IRequirement
    {
        string Description { get; }

        IReadOnlyCollection<IRequirement> DependsOn { get; }

        CollectionInfo? CollectionInfo { get; }

        IReadOnlyCollection<IRequirement> ExclusiveWith { get; }

        Guid Id { get; }

        bool IsOptional { get; }

        string Name { get; }

        IRequirementType Type { get; }

        ISetValidator CreateSetValidator();

        Exception Validate(object value, IConfiguration dependencies);
    }

    public interface IRequirement<T> : IRequirement
    {
        IReadOnlyCollection<SetRestrictionInfo<T>> RestrictedToSet { get; }

        new IRequirementType<T> Type { get; }

        new ISetValidator<T> CreateSetValidator();

        Exception Validate(T value, IConfiguration dependencies);
    }
}
