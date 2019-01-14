using System;
using System.Numerics;
using System.Security;
using Drexel.Configurables.External;
using Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft
{
    public static class StandardTypeSerializers
    {
        static StandardTypeSerializers()
        {
            StandardTypeSerializers.BigInteger = new BigIntegerSerializer(StandardRequirementTypes.BigInteger);
            StandardTypeSerializers.Boolean = new BooleanSerializer(StandardRequirementTypes.Boolean);
            StandardTypeSerializers.DateTime = new DateTimeSerializer(StandardRequirementTypes.DateTime);
            StandardTypeSerializers.Decimal = new DecimalSerializer(StandardRequirementTypes.Decimal);
            StandardTypeSerializers.Double = new DoubleSerializer(StandardRequirementTypes.Double);
            StandardTypeSerializers.FilePath = new FilePathSerializer(StandardRequirementTypes.FilePath);
            StandardTypeSerializers.Int32 = new Int32Serializer(StandardRequirementTypes.Int32);
            StandardTypeSerializers.Int64 = new Int64Serializer(StandardRequirementTypes.Int64);
            StandardTypeSerializers.SecureString = new SecureStringSerializer(StandardRequirementTypes.SecureString);
            StandardTypeSerializers.Single = new SingleSerializer(StandardRequirementTypes.Single);
            StandardTypeSerializers.String = new StringSerializer(StandardRequirementTypes.String);
            StandardTypeSerializers.TimeSpan = new TimeSpanSerializer(StandardRequirementTypes.TimeSpan);
            StandardTypeSerializers.UInt16 = new UInt16Serializer(StandardRequirementTypes.UInt16);
            StandardTypeSerializers.UInt64 = new UInt64Serializer(StandardRequirementTypes.UInt64);
            StandardTypeSerializers.Uri = new UriSerializer(StandardRequirementTypes.Uri);

            TypeSerializerDictionaryBuilder<NewtonsoftIntermediary> builder =
                new TypeSerializerDictionaryBuilder<NewtonsoftIntermediary>()
                    .Add(StandardRequirementTypes.BigInteger, StandardTypeSerializers.BigInteger)
                    .Add(StandardRequirementTypes.Boolean, StandardTypeSerializers.Boolean)
                    .Add(StandardRequirementTypes.DateTime, StandardTypeSerializers.DateTime)
                    .Add(StandardRequirementTypes.Decimal, StandardTypeSerializers.Decimal)
                    .Add(StandardRequirementTypes.Double, StandardTypeSerializers.Double)
                    .Add(StandardRequirementTypes.FilePath, StandardTypeSerializers.FilePath)
                    .Add(StandardRequirementTypes.Int32, StandardTypeSerializers.Int32)
                    .Add(StandardRequirementTypes.Int64, StandardTypeSerializers.Int64)
                    .Add(StandardRequirementTypes.SecureString, StandardTypeSerializers.SecureString)
                    .Add(StandardRequirementTypes.Single, StandardTypeSerializers.Single)
                    .Add(StandardRequirementTypes.String, StandardTypeSerializers.String)
                    .Add(StandardRequirementTypes.TimeSpan, StandardTypeSerializers.TimeSpan)
                    .Add(StandardRequirementTypes.UInt16, StandardTypeSerializers.UInt16)
                    .Add(StandardRequirementTypes.UInt64, StandardTypeSerializers.UInt64)
                    .Add(StandardRequirementTypes.Uri, StandardTypeSerializers.Uri);
            StandardTypeSerializers.StandardSerializers = builder.Build();
        }


        public static IStructTypeSerializer<BigInteger, NewtonsoftIntermediary> BigInteger { get; }

        public static IStructTypeSerializer<Boolean, NewtonsoftIntermediary> Boolean { get; }

        public static IStructTypeSerializer<DateTime, NewtonsoftIntermediary> DateTime { get; }

        public static IStructTypeSerializer<Decimal, NewtonsoftIntermediary> Decimal { get; }

        public static IStructTypeSerializer<Double, NewtonsoftIntermediary> Double { get; }

        public static IClassTypeSerializer<FilePath, NewtonsoftIntermediary> FilePath { get; }

        public static IStructTypeSerializer<Int32, NewtonsoftIntermediary> Int32 { get; }

        public static IStructTypeSerializer<Int64, NewtonsoftIntermediary> Int64 { get; }

        public static IClassTypeSerializer<SecureString, NewtonsoftIntermediary> SecureString { get; }

        public static IStructTypeSerializer<Single, NewtonsoftIntermediary> Single { get; }

        public static IClassTypeSerializer<String, NewtonsoftIntermediary> String { get; }

        public static IStructTypeSerializer<TimeSpan, NewtonsoftIntermediary> TimeSpan { get; }

        public static IStructTypeSerializer<UInt16, NewtonsoftIntermediary> UInt16 { get; }

        public static IStructTypeSerializer<UInt64, NewtonsoftIntermediary> UInt64 { get; }

        public static IClassTypeSerializer<Uri, NewtonsoftIntermediary> Uri { get; }

        public static TypeSerializerDictionary<NewtonsoftIntermediary> StandardSerializers { get; }
    }
}
