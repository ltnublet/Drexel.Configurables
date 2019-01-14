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
    /// Provides access to the standard set of <see cref="RequirementType"/>s. These requirement types are guaranteed
    /// to be supported in all related official libraries (ex. serialization/deserialization or UI frameworks).
    /// </summary>
    public static class StandardRequirementTypes
    {
        static StandardRequirementTypes()
        {
            StandardRequirementTypes.BigInteger = BigIntegerRequirementType.Instance;
            StandardRequirementTypes.Boolean = BooleanRequirementType.Instance;
            StandardRequirementTypes.DateTime = DateTimeRequirementType.Instance;
            StandardRequirementTypes.Decimal = DecimalRequirementType.Instance;
            StandardRequirementTypes.Double = DoubleRequirementType.Instance;
            StandardRequirementTypes.FilePath = FilePathRequirementType.Instance;
            StandardRequirementTypes.Int32 = Int32RequirementType.Instance;
            StandardRequirementTypes.Int64 = Int64RequirementType.Instance;
            StandardRequirementTypes.SecureString = SecureStringRequirementType.Instance;
            StandardRequirementTypes.Single = SingleRequirementType.Instance;
            StandardRequirementTypes.String = StringRequirementType.Instance;
            StandardRequirementTypes.TimeSpan = TimeSpanRequirementType.Instance;
            StandardRequirementTypes.UInt16 = UInt16RequirementType.Instance;
            StandardRequirementTypes.UInt64 = UInt64RequirementType.Instance;
            StandardRequirementTypes.Uri = UriRequirementType.Instance;

            StandardRequirementTypes.StandardTypes = new ReadOnlyCollection<RequirementType>(
                new List<RequirementType>()
                {
                    StandardRequirementTypes.BigInteger,
                    StandardRequirementTypes.Boolean,
                    StandardRequirementTypes.DateTime,
                    StandardRequirementTypes.Decimal,
                    StandardRequirementTypes.Double,
                    StandardRequirementTypes.FilePath,
                    StandardRequirementTypes.Int32,
                    StandardRequirementTypes.Int64,
                    StandardRequirementTypes.SecureString,
                    StandardRequirementTypes.Single,
                    StandardRequirementTypes.String,
                    StandardRequirementTypes.TimeSpan,
                    StandardRequirementTypes.UInt16,
                    StandardRequirementTypes.UInt64,
                    StandardRequirementTypes.Uri
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
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.DateTime"/>.
        /// </summary>
        public static StructRequirementType<DateTime> DateTime { get; }

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
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.TimeSpan"/>.
        /// </summary>
        public static StructRequirementType<TimeSpan> TimeSpan { get; }

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
        /// Gets the set of standard <see cref="RequirementType"/>s exposed by this class.
        /// </summary>
        public static IReadOnlyCollection<RequirementType> StandardTypes { get; }
    }
}
