using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.RequirementTypes
{
    public abstract class ClassRequirementTypeBase<T> : IClassRequirementType<T>
        where T : class
    {
        public ClassRequirementTypeBase(
            Guid id,
            bool persistable,
            Version version)
        {
            this.Id = id;
            this.IsPersistable = persistable;
            this.Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public Guid Id { get; }

        public bool IsPersistable { get; }

        public Type Type => typeof(T);

        public Version Version { get; }

        public abstract T? Cast(object? value);

        public string? Persist(T? value)
        {
            this.ThrowIfNotPersistable();
            return this.PersistInternal(value);
        }

        public string? Persist(object? value)
        {
            this.ThrowIfNotPersistable();
            return this.PersistInternal(this.Cast(value));
        }

        public T? Restore(string? value)
        {
            this.ThrowIfNotPersistable();
            return this.RestoreInternal(value);
        }

        object? IRequirementType.Restore(string? value) => this.Restore(value);

        protected abstract string? PersistInternal(T? value);

        protected abstract T? RestoreInternal(string? value);

        [System.Diagnostics.DebuggerHidden]
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void ThrowIfNotPersistable()
        {
            if (!this.IsPersistable)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
