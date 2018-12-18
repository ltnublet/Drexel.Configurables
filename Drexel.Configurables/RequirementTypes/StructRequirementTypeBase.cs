using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.RequirementTypes
{
    public abstract class StructRequirementTypeBase<T> : IStructRequirementType<T>
        where T : struct
    {
        public StructRequirementTypeBase(
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

        public T Cast(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return this.CastInternal(value);
        }

        public string Persist(T value)
        {
            this.ThrowIfNotPersistable();
            return this.PersistInternal(value);
        }

        public string? Persist(object? value)
        {
            this.ThrowIfNotPersistable();
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return this.PersistInternal(this.Cast(value));
        }

        public T Restore(string value)
        {
            this.ThrowIfNotPersistable();
            return this.RestoreInternal(value);
        }

        object? IRequirementType.Restore(string? value)
        {
            this.ThrowIfNotPersistable();
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return this.Restore(value);
        }

        protected abstract T CastInternal(object value);

        protected abstract string PersistInternal(T value);

        protected abstract T RestoreInternal(string value);

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
