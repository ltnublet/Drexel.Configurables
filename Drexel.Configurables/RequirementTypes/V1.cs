using System;
using System.Globalization;
using System.Security;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;

namespace Drexel.Configurables.RequirementTypes
{
    public static class V1
    {
        public static IRequirementType<bool> Bool { get; } = new BoolRequirementType();

        public static IRequirementType<int> Int32 { get; } = new Int32RequirementType();

        public static IRequirementType<long> Int64 { get; } = new Int64RequirementType();

        public static IRequirementType<SecureString> SecureString { get; } = new SecureStringRequirementType();

        public static IRequirementType<string> String { get; } = new StringRequirementType();

        public static IRequirementType<Uri> Uri { get; } = new UriRequirementType();

        public static IRequirementType<FilePath> GetFilePathType(IPathInteractor pathInteractor)
        {
            return new FilePathRequirementType(pathInteractor);
        }

        public static bool IsDefaultType(IRequirementType requirementType)
        {
            if (requirementType == null)
            {
                throw new ArgumentNullException(nameof(requirementType));
            }

            Type underlyingType = requirementType.GetType();
            return underlyingType == typeof(BoolRequirementType)
                || underlyingType == typeof(FilePathRequirementType)
                || underlyingType == typeof(Int32RequirementType)
                || underlyingType == typeof(Int64RequirementType)
                || underlyingType == typeof(SecureStringRequirementType)
                || underlyingType == typeof(StringRequirementType)
                || underlyingType == typeof(UriRequirementType);
        }

        private sealed class BoolRequirementType : RequirementType<bool>
        {
            public BoolRequirementType()
                : base("2af45e35-7067-4d4a-aaf3-3ffa7cfc23fc", true)
            {
                // Nothing to do.
            }

            public override bool Cast(object value)
            {
                if (value is bool asBool)
                {
                    return asBool;
                }

                throw new InvalidCastException();
            }

            protected override string PersistInternal(bool value) => value.ToString(CultureInfo.InvariantCulture);

            protected override bool RestoreInternal(string value)
            {
                if (bool.TryParse(value, out bool result))
                {
                    return result;
                }

                throw new InvalidCastException();
            }
        }

        private sealed class FilePathRequirementType : RequirementType<FilePath>
        {
            private readonly IPathInteractor pathInteractor;

            public FilePathRequirementType(IPathInteractor pathInteractor)
                : base("54ce6ec8-f6dc-4fd3-87bd-56b409d3f5bc", true)
            {
                this.pathInteractor = pathInteractor;
            }

            public override FilePath Cast(object value)
            {
                if (value is FilePath asPath)
                {
                    return asPath;
                }

                throw new InvalidCastException();
            }

            protected override string PersistInternal(FilePath value)
            {
                if (value == null)
                {
                    throw new InvalidCastException();
                }

                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}:{1}",
                    value.CaseSensitive,
                    value.Path);
            }

            protected override FilePath RestoreInternal(string value)
            {
                if (value != null)
                {
                    int dividerIndex = value.IndexOf(':');
                    if (dividerIndex > -1)
                    {
                        if (bool.TryParse(value.Substring(0, dividerIndex), out bool caseSensitive))
                        {
                            return new FilePath(
                                value.Substring(dividerIndex),
                                this.pathInteractor,
                                caseSensitive);
                        }
                    }
                }

                throw new InvalidCastException();
            }
        }

        private sealed class Int32RequirementType : RequirementType<int>
        {
            public Int32RequirementType()
                : base("d3fcd5ad-fbca-4ccd-826f-81c855f99acc", true)
            {
                // Nothing to do.
            }

            public override int Cast(object value)
            {
                if (value is int asInt)
                {
                    return asInt;
                }

                throw new InvalidCastException();
            }

            protected override string PersistInternal(int value) => value.ToString(CultureInfo.InvariantCulture);

            protected override int RestoreInternal(string value)
            {
                if (int.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int result))
                {
                    return result;
                }

                throw new InvalidCastException();
            }
        }

        private sealed class Int64RequirementType : RequirementType<long>
        {
            public Int64RequirementType()
                : base("f4d9529c-d8b1-4676-ab5c-4989b66679ed", true)
            {
                // Nothing to do.
            }

            public override long Cast(object value)
            {
                if (value is long asLong)
                {
                    return asLong;
                }

                throw new InvalidCastException();
            }

            protected override string PersistInternal(long value) => value.ToString(CultureInfo.InvariantCulture);

            protected override long RestoreInternal(string value)
            {
                if (long.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out long result))
                {
                    return result;
                }

                throw new InvalidCastException();
            }
        }

        private sealed class SecureStringRequirementType : RequirementType<SecureString>
        {
            public SecureStringRequirementType()
                : base("b4213094-5ab8-43f7-8fad-dcfe7d08d47d", false)
            {
                // Nothing to do.
            }

            public override SecureString Cast(object value)
            {
                if (value is SecureString asSecureString)
                {
                    return asSecureString;
                }

                throw new InvalidCastException();
            }

            protected override string PersistInternal(SecureString value)
            {
                throw new NotImplementedException();
            }

            protected override SecureString RestoreInternal(string value)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class StringRequirementType : RequirementType<string>
        {
            public StringRequirementType()
                : base("b97c3d45-cde4-4001-bbe6-3087da22acd5", true)
            {
                // Nothing to do.
            }

            public override string Cast(object value)
            {
                if (value is string asString)
                {
                    return asString;
                }

                throw new InvalidCastException();
            }

            protected override string PersistInternal(string value) => value;

            protected override string RestoreInternal(string value) => value;
        }

        private sealed class UriRequirementType : RequirementType<Uri>
        {
            public UriRequirementType()
                : base("7d40ee53-81ec-4714-9e04-e9e8f430b361", true)
            {
                // Nothing to do.
            }

            public override Uri Cast(object value)
            {
                if (value is Uri asUri)
                {
                    return asUri;
                }

                throw new InvalidCastException();
            }

            protected override string PersistInternal(Uri value)
            {
                if (value == null)
                {
                    throw new InvalidCastException();
                }

                return value.OriginalString;
            }

            protected override Uri RestoreInternal(string value)
            {
                if (value != null)
                {
                    return new Uri(value);
                }

                throw new InvalidCastException();
            }
        }

        private abstract class RequirementType<T> : IRequirementType<T>
        {
            public RequirementType(string id, bool persistable)
            {
                this.Id = Guid.Parse(id);
                this.IsPersistable = persistable;
                this.Version = new Version(1, 0, 0, 0);
            }

            public Guid Id { get; }

            public bool IsPersistable { get; }

            public Type Type => typeof(T);

            public Version Version { get; }

            public abstract T Cast(object value);

            public string Persist(T value)
            {
                this.ThrowIfNotPersistable();
                return this.PersistInternal(value);
            }

            public string Persist(object value)
            {
                this.ThrowIfNotPersistable();
                return this.PersistInternal(this.Cast(value));
            }

            public T Restore(string value)
            {
                this.ThrowIfNotPersistable();
                return this.RestoreInternal(value);
            }

            object IRequirementType.Restore(string value) => this.Restore(value);

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
}
