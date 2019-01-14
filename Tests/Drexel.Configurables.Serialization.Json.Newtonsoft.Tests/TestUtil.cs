using System;
using System.Collections;
using System.Numerics;
using System.Security;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.Tests
{
    public static class TestUtil
    {
        public static Requirement GetValidRequirement(this RequirementType requirementType, bool collection)
        {
            Type type = requirementType.Type;
            CollectionInfo? info = collection ? new CollectionInfo() : (CollectionInfo?)null;

            if (type == typeof(BigInteger))
            {
                return StandardRequirements.CreateBigInteger(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(Boolean))
            {
                return StandardRequirements.CreateBoolean(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(DateTime))
            {
                return StandardRequirements.CreateDateTime(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(Decimal))
            {
                return StandardRequirements.CreateDecimal(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(Double))
            {
                return StandardRequirements.CreateDouble(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(FilePath))
            {
                return StandardRequirements.CreateFilePath(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(Int32))
            {
                return StandardRequirements.CreateInt32(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(Int64))
            {
                return StandardRequirements.CreateInt64(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(SecureString))
            {
                return StandardRequirements.CreateSecureString(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(Single))
            {
                return StandardRequirements.CreateSingle(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(String))
            {
                return StandardRequirements.CreateString(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(TimeSpan))
            {
                return StandardRequirements.CreateTimeSpan(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(UInt16))
            {
                return StandardRequirements.CreateUInt16(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(UInt64))
            {
                return StandardRequirements.CreateUInt64(Guid.NewGuid(), collectionInfo: info);
            }
            else if (type == typeof(Uri))
            {
                return StandardRequirements.CreateUri(Guid.NewGuid(), collectionInfo: info);
            }
            else
            {
                throw new InternalTestFailureException("TEST FAILURE: unanticipated requirement type.");
            }
        }

        public static object? GetValidValue(this RequirementType requirementType)
        {
            Type type = requirementType.Type;
            if (type == typeof(BigInteger))
            {
                return new BigInteger(1111111);
            }
            else if (type == typeof(Boolean))
            {
                return true;
            }
            else if (type == typeof(DateTime))
            {
                return new DateTime(2018, 1, 2, 12, 30, 15);
            }
            else if (type == typeof(Decimal))
            {
                return new Decimal(22222222);
            }
            else if (type == typeof(Double))
            {
                return 8.675309D;
            }
            else if (type == typeof(FilePath))
            {
                return new FilePath(@"C:\Foo\Bar", MockPathInteractor.Instance, true);
            }
            else if (type == typeof(Int32))
            {
                return 333333;
            }
            else if (type == typeof(Int64))
            {
                return 4444444444444L;
            }
            else if (type == typeof(SecureString))
            {
                return "Hello, world".ToSecureString();
            }
            else if (type == typeof(Single))
            {
                return 867.5309F;
            }
            else if (type == typeof(String))
            {
                return "Hello, world";
            }
            else if (type == typeof(TimeSpan))
            {
                return new TimeSpan(8, 6, 7, 5, 3);
            }
            else if (type == typeof(UInt16))
            {
                return (UInt16)8675;
            }
            else if (type == typeof(UInt64))
            {
                return 123456789123456789UL;
            }
            else if (type == typeof(Uri))
            {
                return new Uri("https://www.example.com/foo?bar=baz#bang");
            }
            else
            {
                throw new InternalTestFailureException("TEST FAILURE: unanticipated requirement type.");
            }
        }

        public static IEnumerable? GetValidCollection(this RequirementType requirementType)
        {
            Type type = requirementType.Type;
            if (type == typeof(BigInteger))
            {
                return new BigInteger[]
                {
                        new BigInteger(1111111),
                        new BigInteger(1111112),
                        new BigInteger(1111113)
                };
            }
            else if (type == typeof(Boolean))
            {
                return new bool[]
                {
                        true,
                        false,
                        true
                };
            }
            else if (type == typeof(DateTime))
            {
                return new DateTime[]
                {
                        new DateTime(2018, 1, 2, 12, 30, 15),
                        new DateTime(2018, 1, 3, 12, 30, 15),
                        new DateTime(2018, 1, 4, 12, 30, 15)
                };
            }
            else if (type == typeof(Decimal))
            {
                return new Decimal[]
                {
                        new Decimal(22222222),
                        new Decimal(22222223),
                        new Decimal(22222224)
                };
            }
            else if (type == typeof(Double))
            {
                return new Double[]
                {
                        8.675309D,
                        8.675310D,
                        8.675311D,
                };
            }
            else if (type == typeof(FilePath))
            {
                return new FilePath[]
                {
                        new FilePath(@"C:\Foo\Bar", MockPathInteractor.Instance, true),
                        new FilePath(@"A:\B\C", MockPathInteractor.Instance, true),
                        new FilePath(@"E:\F\G", MockPathInteractor.Instance, true)
                };
            }
            else if (type == typeof(Int32))
            {
                return new int[]
                {
                        333333,
                        333334,
                        333335
                };
            }
            else if (type == typeof(Int64))
            {
                return new Int64[]
                {
                        4444444444444L,
                        4444444444445L,
                        4444444444446L
                };
            }
            else if (type == typeof(SecureString))
            {
                return new SecureString[]
                {
                        "Hello, world".ToSecureString(),
                        "Hello world!".ToSecureString(),
                        "Hello... world?".ToSecureString()
                };
            }
            else if (type == typeof(Single))
            {
                return new Single[]
                {
                        867.5309F,
                        867.5310F,
                        867.5311F
                };
            }
            else if (type == typeof(String))
            {
                return new string[]
                {
                        "Hello, world",
                        "Hello world!",
                        "Hello... world?"
                };
            }
            else if (type == typeof(TimeSpan))
            {
                return new TimeSpan[]
                {
                        new TimeSpan(8, 6, 7, 5, 3),
                        new TimeSpan(9, 7, 8, 6, 4),
                        new TimeSpan(7, 5, 6, 4, 2)
                };
            }
            else if (type == typeof(UInt16))
            {
                return new UInt16[]
                {
                        (UInt16)8675,
                        (UInt16)8676,
                        (UInt16)8677
                };
            }
            else if (type == typeof(UInt64))
            {
                return new UInt64[]
                {
                        123456789123456789UL,
                        123456789123456790UL,
                        123456789123456791UL
                };
            }
            else if (type == typeof(Uri))
            {
                return new Uri[]
                {
                        new Uri("https://www.example.com/foo?bar=baz#bang"),
                        new Uri("ftpes://www.example2.com/"),
                        new Uri("https://www.example3.com/hello")
                };
            }
            else
            {
                throw new InternalTestFailureException("TEST FAILURE: unanticipated requirement type.");
            }
        }
    }
}
