using System;
using System.Globalization;
using System.Security;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;

namespace Drexel.Configurables.RequirementTypes
{
    public static class V1
    {
        private static readonly Version Version = new Version(1, 0, 0, 0);

        public static IStructRequirementType<bool> Bool { get; } = new BoolRequirementType();

        public static IStructRequirementType<int> Int32 { get; } = new Int32RequirementType();

        public static IStructRequirementType<long> Int64 { get; } = new Int64RequirementType();

        public static IClassRequirementType<SecureString> SecureString { get; } = new SecureStringRequirementType();

        public static IClassRequirementType<string> String { get; } = new StringRequirementType();

        public static IClassRequirementType<Uri> Uri { get; } = new UriRequirementType();

        public static IClassRequirementType<FilePath> GetFilePathType(IPathInteractor pathInteractor)
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

        private sealed class BoolRequirementType : StructRequirementTypeBase<bool>
        {
            private static readonly Guid BoolRequirementTypeId = Guid.Parse("2af45e35-7067-4d4a-aaf3-3ffa7cfc23fc");

            public BoolRequirementType()
                : base(
                      BoolRequirementType.BoolRequirementTypeId,
                      true,
                      V1.Version)
            {
                // Nothing to do.
            }

            protected override bool CastInternal(object value)
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

        private sealed class FilePathRequirementType : ClassRequirementTypeBase<FilePath>
        {
            private static readonly Guid FilePathRequirementTypeId =
                Guid.Parse("54ce6ec8-f6dc-4fd3-87bd-56b409d3f5bc");

            private readonly IPathInteractor pathInteractor;

            public FilePathRequirementType(IPathInteractor pathInteractor)
                : base(
                      FilePathRequirementType.FilePathRequirementTypeId,
                      true,
                      V1.Version)
            {
                this.pathInteractor = pathInteractor;
            }

            public override FilePath? Cast(object? value)
            {
                if (value == null)
                {
                    return null;
                }
                else if (value is FilePath asPath)
                {
                    return asPath;
                }

                throw new InvalidCastException();
            }

            protected override string? PersistInternal(FilePath? value)
            {
                if (value == null)
                {
                    return null;
                }

                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}:{1}",
                    value.CaseSensitive,
                    value.Path);
            }

            protected override FilePath? RestoreInternal(string? value)
            {
                if (value == null)
                {
                    return null;
                }

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

                throw new InvalidCastException();
            }
        }

        private sealed class Int32RequirementType : StructRequirementTypeBase<int>
        {
            private static readonly Guid Int32RequirementTypeId = Guid.Parse("d3fcd5ad-fbca-4ccd-826f-81c855f99acc");

            public Int32RequirementType()
                : base(
                      Int32RequirementType.Int32RequirementTypeId,
                      true,
                      V1.Version)
            {
                // Nothing to do.
            }

            protected override int CastInternal(object value)
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

        private sealed class Int64RequirementType : StructRequirementTypeBase<long>
        {
            private static readonly Guid Int64RequirementTypeId = Guid.Parse("f4d9529c-d8b1-4676-ab5c-4989b66679ed");

            public Int64RequirementType()
                : base(
                      Int64RequirementType.Int64RequirementTypeId,
                      true,
                      V1.Version)
            {
                // Nothing to do.
            }

            protected override long CastInternal(object value)
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

        private sealed class SecureStringRequirementType : ClassRequirementTypeBase<SecureString>
        {
            private static readonly Guid SecureStringRequirementTypeId =
                Guid.Parse("b4213094-5ab8-43f7-8fad-dcfe7d08d47d");

            public SecureStringRequirementType()
                : base(
                      SecureStringRequirementType.SecureStringRequirementTypeId,
                      false,
                      V1.Version)
            {
                // Nothing to do.
            }

            public override SecureString? Cast(object? value)
            {
                if (value == null)
                {
                    return null;
                }
                else if (value is SecureString asSecureString)
                {
                    return asSecureString;
                }

                throw new InvalidCastException();
            }

            protected override string? PersistInternal(SecureString? value)
            {
                throw new NotImplementedException();
            }

            protected override SecureString? RestoreInternal(string? value)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class StringRequirementType : ClassRequirementTypeBase<string>
        {
            private static readonly Guid StringRequirementTypeId = Guid.Parse("b97c3d45-cde4-4001-bbe6-3087da22acd5");

            public StringRequirementType()
                : base(
                      StringRequirementType.StringRequirementTypeId,
                      true,
                      V1.Version)
            {
                // Nothing to do.
            }

            public override string? Cast(object? value)
            {
                if (value == null)
                {
                    return null;
                }
                else if (value is string asString)
                {
                    return asString;
                }

                throw new InvalidCastException();
            }

            protected override string? PersistInternal(string? value) => value;

            protected override string? RestoreInternal(string? value) => value;
        }

        private sealed class UriRequirementType : ClassRequirementTypeBase<Uri>
        {
            private static readonly Guid UriRequirementTypeId = Guid.Parse("7d40ee53-81ec-4714-9e04-e9e8f430b361");

            public UriRequirementType()
                : base(
                      UriRequirementType.UriRequirementTypeId,
                      true,
                      V1.Version)
            {
                // Nothing to do.
            }

            public override Uri? Cast(object? value)
            {
                if (value == null)
                {
                    return null;
                }
                else if (value is Uri asUri)
                {
                    return asUri;
                }

                throw new InvalidCastException();
            }

            protected override string? PersistInternal(Uri? value)
            {
                if (value == null)
                {
                    return null;
                }

                return value.OriginalString;
            }

            protected override Uri? RestoreInternal(string? value)
            {
                if (value == null)
                {
                    return null;
                }

                return new Uri(value);
            }
        }
    }
}
