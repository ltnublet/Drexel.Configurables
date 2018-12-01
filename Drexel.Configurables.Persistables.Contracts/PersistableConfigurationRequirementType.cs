using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Represents a persistable configuration type.
    /// </summary>
    public class PersistableConfigurationRequirementType : IConfigurationRequirementType
    {
        private readonly IConfigurationRequirementType type;
        private readonly Func<object, string> encodeFunc;
        private readonly Func<string, object> restoreFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistableConfigurationRequirementType"/> class.
        /// </summary>
        /// <param name="id">
        /// The unique ID that identifies this requirement type.
        /// </param>
        /// <param name="version">
        /// The <see cref="Version"/> of the requirement type.
        /// </param>
        /// <param name="type">
        /// The underlying <see cref="IConfigurationRequirementType"/>.
        /// </param>
        /// <param name="encodeFunc">
        /// The encoding function to convert a value of this type to a <see langword="string"/>.
        /// </param>
        /// <param name="restoreFunc">
        /// The restoration function to convert a <see langword="string"/> to a value of this type.
        /// </param>
        public PersistableConfigurationRequirementType(
            Guid id,
            Version version,
            IConfigurationRequirementType type,
            Func<object, string> encodeFunc,
            Func<string, object> restoreFunc)
        {
            this.Id = id;
            this.Version = version ?? throw new ArgumentNullException(nameof(version));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.encodeFunc = encodeFunc ?? throw new ArgumentNullException(nameof(encodeFunc));
            this.restoreFunc = restoreFunc ?? throw new ArgumentNullException(nameof(restoreFunc));
        }

        /// <summary>
        /// Gets the unique ID of this requirement type.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the associated <see cref="IConfigurationRequirementType"/>.
        /// </summary>
        public Type Type => this.type.Type;

        /// <summary>
        /// Gets the <see cref="System.Version"/> of this requirement type.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Encodes the supplied <see langword="object"/> <paramref name="value"/> to a <see langword="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see langword="object"/> value to encode.
        /// </param>
        /// <returns>
        /// A <see langword="string"/> encoding of the supplied <see langword="object"/> value.
        /// </returns>
        public string Encode(object value) => this.encodeFunc.Invoke(value);

        /// <summary>
        /// Restores the supplied <see langword="string"/> <paramref name="value"/> to an <see langword="object"/>
        /// of the <see cref="System.Type"/> of this <see cref="IConfigurationRequirementType"/>.
        /// </summary>
        /// <param name="value">
        /// The <see langword="string"/> <paramref name="value"/> to restore.
        /// </param>
        /// <returns>
        /// An <see langword="object"/> of the <see langword="System.Type"/> of this
        /// <see cref="IConfigurationRequirementType"/>.
        /// </returns>
        public object Restore(string value) => this.restoreFunc.Invoke(value);

        public static class V1
        {
            private static readonly IPathInteractor PathInteractorSingleton = new AlwaysAllowInteractor();

            /// <summary>
            /// Initializes static members of the <see cref="PersistableConfigurationRequirementType"/> class. Populates
            /// properties in order, so that reflection can be used to retrieve the total set of supported types.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage(
                "Microsoft.Performance",
                "CA1810:InitializeReferenceTypeStaticFieldsInline",
                Justification = "Order of initializations matters.")]
            static V1()
            {
                const string boolId = "01e3bc15-2be9-4798-a49d-50d96e31d783";
                const string filePathId = "02be53fd-6561-4350-be67-f5fa1ab2edb0";
                const string int32Id = "967cc980-0f7a-433a-b594-4bfdbee2847b";
                const string int64Id = "c8622e5c-7552-42ec-9b60-d0236f65cfe5";
                const string stringId = "77f5404d-eef7-4749-927d-70a0d714c27a";
                const string uriId = "4e6939c2-b298-4030-a11e-660814294400";

                const string boolVersion = "1.0.0.0";
                const string filePathVersion = "1.0.0.0";
                const string int32Version = "1.0.0.0";
                const string int64Version = "1.0.0.0";
                const string stringVersion = "1.0.0.0";
                const string uriVersion = "1.0.0.0";

#pragma warning disable SA1121 // Use built-in type alias
                V1.Bool =
                    new PersistableConfigurationRequirementType(
                        Guid.Parse(boolId),
                        Version.Parse(boolVersion),
                        ConfigurationRequirementType.Bool,
                        V1.BoolEncode,
                        V1.BoolDecode);
                V1.FilePath =
                    new PersistableConfigurationRequirementType(
                        Guid.Parse(filePathId),
                        Version.Parse(filePathVersion),
                        ConfigurationRequirementType.FilePath,
                        V1.FilePathEncode,
                        V1.FilePathDecode);
                V1.Int32 =
                    new PersistableConfigurationRequirementType(
                        Guid.Parse(int32Id),
                        Version.Parse(int32Version),
                        ConfigurationRequirementType.Int32,
                        V1.Int32Encode,
                        V1.Int32Decode);
                V1.Int64 =
                    new PersistableConfigurationRequirementType(
                        Guid.Parse(int64Id),
                        Version.Parse(int64Version),
                        ConfigurationRequirementType.Int64,
                        V1.Int64Encode,
                        V1.Int64Decode);
                V1.String =
                    new PersistableConfigurationRequirementType(
                        Guid.Parse(stringId),
                        Version.Parse(stringVersion),
                        ConfigurationRequirementType.String,
                        V1.StringEncode,
                        V1.StringDecode);
                V1.Uri =
                    new PersistableConfigurationRequirementType(
                        Guid.Parse(uriId),
                        Version.Parse(uriVersion),
                        ConfigurationRequirementType.Uri,
                        V1.UriEncode,
                        V1.UriDecode);
#pragma warning restore SA1121 // Use built-in type alias

                // Programmatically build the list of supported types by iterating over all public static fields which are
                // of type ConfigurationRequirementType. All properties must be initialized before this is executed.
                V1.Types =
                    typeof(V1)
                        .GetProperties(BindingFlags.Public | BindingFlags.Static)
                        .Where(x => x.PropertyType == typeof(PersistableConfigurationRequirementType))
                        .Select(x => x.GetValue(null, null))
                        .Cast<PersistableConfigurationRequirementType>()
                        .ToList();
            }

            public static PersistableConfigurationRequirementType Bool { get; }

            public static PersistableConfigurationRequirementType FilePath { get; }

            public static PersistableConfigurationRequirementType Int32 { get; }

            public static PersistableConfigurationRequirementType Int64 { get; }

            public static PersistableConfigurationRequirementType String { get; }

            public static PersistableConfigurationRequirementType Uri { get; }

            public static IReadOnlyList<PersistableConfigurationRequirementType> Types { get; }

            internal static string BoolEncode(object value)
            {
                if (value is bool asBool)
                {
                    return asBool ? "true" : "false";
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            internal static object BoolDecode(string value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(value);
                }

                switch (value.ToUpperInvariant())
                {
                    case "TRUE":
                        return true;
                    case "FALSE":
                        return false;
                    default:
                        throw new InvalidCastException();
                }
            }

            internal static string FilePathEncode(object value)
            {
                if (value is FilePath asPath)
                {
                    return V1.BoolEncode(asPath.CaseSensitive) + ":" + V1.StringEncode(asPath.Path);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            internal static object FilePathDecode(string value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                int delimiterIndex = value.IndexOf(':');
                string caseSensitive = value.Substring(0, delimiterIndex);
                string path = value.Substring(delimiterIndex);

                return new FilePath(
                    (string)V1.StringDecode(path),
                    V1.PathInteractorSingleton, // TODO: What if someone manually edits value, and now it's not rooted?
                    (bool)V1.BoolDecode(caseSensitive));
            }

            internal static string Int32Encode(object value)
            {
                if (value is Int32 asInt)
                {
                    return asInt.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            internal static object Int32Decode(string value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return int.Parse(value, CultureInfo.InvariantCulture);
            }

            internal static string Int64Encode(object value)
            {
                if (value is System.Int64 asLong)
                {
                    return asLong.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            internal static object Int64Decode(string value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return System.Int64.Parse(value, CultureInfo.InvariantCulture);
            }

            internal static string StringEncode(object value)
            {
                if (value is string asString)
                {
                    return asString;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            internal static object StringDecode(string value)
            {
                return value;
            }

            internal static string UriEncode(object value)
            {
                if (value is Uri asUri)
                {
                    return asUri.AbsoluteUri;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            internal static object UriDecode(string value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                // TODO: Does this break URIs with escaped characters?
                UriBuilder builder = new UriBuilder(value);
                return builder.Uri;
            }

            private class AlwaysAllowInteractor : IPathInteractor
            {
                public string GetFullPath(string path) => path;

                public bool IsPathRooted(string path) => true;
            }
        }
    }
}
