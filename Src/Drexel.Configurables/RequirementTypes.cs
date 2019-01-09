using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Security;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;
using Drexel.Configurables.External;
using Drexel.Configurables.Internals.Types;

namespace Drexel.Configurables
{
    /// <summary>
    /// Provides access to the default set of <see cref="RequirementType"/>s. These requirement types are guaranteed
    /// to be supported in all related official libraries (ex. serialization/deserialization or UI frameworks).
    /// </summary>
    public static class RequirementTypes
    {
        static RequirementTypes()
        {
            RequirementTypes.BigInteger = BigIntegerRequirementType.Instance;
            RequirementTypes.Boolean = BooleanRequirementType.Instance;
            RequirementTypes.Decimal = DecimalRequirementType.Instance;
            RequirementTypes.Double = DoubleRequirementType.Instance;
            RequirementTypes.FilePath = FilePathRequirementType.Instance;
            RequirementTypes.Int32 = Int32RequirementType.Instance;
            RequirementTypes.Int64 = Int64RequirementType.Instance;
            RequirementTypes.SecureString = SecureStringRequirementType.Instance;
            RequirementTypes.Single = SingleRequirementType.Instance;
            RequirementTypes.String = StringRequirementType.Instance;
            RequirementTypes.UInt16 = UInt16RequirementType.Instance;
            RequirementTypes.UInt64 = UInt64RequirementType.Instance;
            RequirementTypes.Uri = UriRequirementType.Instance;

            RequirementTypes.DefaultSupported = new ReadOnlyCollection<RequirementType>(
                new List<RequirementType>()
                {
                    RequirementTypes.BigInteger,
                    RequirementTypes.Boolean,
                    RequirementTypes.Decimal,
                    RequirementTypes.Double,
                    RequirementTypes.FilePath,
                    RequirementTypes.Int32,
                    RequirementTypes.Int64,
                    RequirementTypes.SecureString,
                    RequirementTypes.Single,
                    RequirementTypes.String,
                    RequirementTypes.UInt16,
                    RequirementTypes.UInt64,
                    RequirementTypes.Uri
                });
        }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Numerics.BigInteger"/>.
        /// </summary>
        public static StructRequirementType<BigInteger> BigInteger { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Boolean"/>.
        /// </summary>
        public static StructRequirementType<Boolean> Boolean { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Decimal"/>.
        /// </summary>
        public static StructRequirementType<Decimal> Decimal { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Boolean"/>.
        /// </summary>
        public static StructRequirementType<Double> Double { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="External.FilePath"/>.
        /// </summary>
        public static ClassRequirementType<FilePath> FilePath { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Int32"/>.
        /// </summary>
        public static StructRequirementType<Int32> Int32 { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Int64"/>.
        /// </summary>
        public static StructRequirementType<Int64> Int64 { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Security.SecureString"/>.
        /// </summary>
        public static ClassRequirementType<SecureString> SecureString { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Single"/>.
        /// </summary>
        public static StructRequirementType<Single> Single { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.String"/>.
        /// </summary>
        public static ClassRequirementType<String> String { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.UInt16"/>.
        /// </summary>
        public static StructRequirementType<UInt16> UInt16 { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.UInt64"/>.
        /// </summary>
        public static StructRequirementType<UInt64> UInt64 { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Uri"/>.
        /// </summary>
        public static ClassRequirementType<Uri> Uri { get; }

        /// <summary>
        /// Gets the set of default <see cref="RequirementType"/>s exposed by this class.
        /// </summary>
        public static IReadOnlyCollection<RequirementType> DefaultSupported { get; }
    }
}
