using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Security;
using System.Text;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;
using Drexel.Configurables.External;

namespace Drexel.Configurables
{
    public static class Types
    {
        static Types()
        {
            Types.DefaultSupported = new ReadOnlyCollection<RequirementType>(
                new List<RequirementType>()
                {
                    Types.BigInteger,
                    Types.Boolean,
                    Types.Decimal,
                    Types.Double,
                    Types.FilePath,
                    Types.Int32,
                    Types.Int64,
                    Types.SecureString,
                    Types.Single,
                    Types.String,
                    Types.UInt64,
                    Types.Uri
                });
        }

        public static StructRequirementType<BigInteger> BigInteger { get; }

        public static StructRequirementType<Boolean> Boolean { get; }

        public static StructRequirementType<Decimal> Decimal { get; }

        public static StructRequirementType<Double> Double { get; }

        public static ClassRequirementType<FilePath> FilePath { get; }

        public static StructRequirementType<Int32> Int32 { get; }

        public static StructRequirementType<Int64> Int64 { get; }

        public static ClassRequirementType<SecureString> SecureString { get; }

        public static StructRequirementType<Single> Single { get; }

        public static ClassRequirementType<String> String { get; }

        public static StructRequirementType<UInt64> UInt64 { get; }

        public static ClassRequirementType<Uri> Uri { get; }

        public static IReadOnlyCollection<RequirementType> DefaultSupported { get; }
    }
}
