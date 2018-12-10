using System;

namespace Drexel.Configurables.Contracts
{
    public interface IRequirementType
    {
        Guid Id { get; }

        bool IsPersistable { get; }

        Type Type { get; }

        Version Version { get; }

        string Persist(object value);

        object Restore(string value);
    }

    public interface IRequirementType<T> : IRequirementType
    {
        T Cast(object value);

        string Persist(T value);

        T Restore(string value);
    }
}
