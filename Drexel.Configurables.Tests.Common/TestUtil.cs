using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;
using Drexel.Configurables.Tests.Common.Mocks;

namespace Drexel.Configurables.Tests.Common
{
    public static class TestUtil
    {
        private static Dictionary<IRequirementType, object> defaultValidObjects =
            new Dictionary<IRequirementType, object>()
            {
                [RequirementTypes.V1.Bool] =
                    true,
                [RequirementTypes.V1.Int32] =
                    8675309,
                [RequirementTypes.V1.Int64] =
                    8675309L,
                [RequirementTypes.V1.SecureString] =
                    "Hello world".ToSecureString(),
                [RequirementTypes.V1.String] =
                    "Hello world",
                [RequirementTypes.V1.Uri] =
                    new Uri("https://www.example.com/route/information?parameter1=value1&parameter2=value2")
            };

        private static Random random = new Random();

        private static long counter = 0;

        public static IRequirement CreateRequirement<T>(
            IRequirementType<T> type,
            Guid? id = null,
            string baseName = "ConfigurationRequirementName",
            string baseDescription = "Configuration requirement description.",
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<SetRestrictionInfo<T>> restrictedToSet = null,
            IReadOnlyCollection<IRequirement> dependsOn = null,
            IReadOnlyCollection<IRequirement> exclusiveWith = null,
            Func<T, IConfiguration, Exception> validator = null)
        {
            return new Requirement<T>(
                id.HasValue ? id.Value : Guid.NewGuid(),
                baseName + TestUtil.counter++,
                baseDescription + TestUtil.counter++,
                type,
                isOptional,
                collectionInfo,
                restrictedToSet,
                dependsOn,
                exclusiveWith,
                validator);
        }

        public static IReadOnlyCollection<IRequirement> CreateRequirementCollection(
            int count,
            bool randomTypes = false,
            bool areOptional = false) =>
            Enumerable
                .Range(0, count)
                .Select(
                    x =>
                    {
                        if (randomTypes)
                        {
                            switch (TestUtil.random.Next(0, 7))
                            {
                                case 0:
                                    return TestUtil.CreateRequirement(
                                        RequirementTypes.V1.Bool,
                                        isOptional: areOptional);
                                case 1:
                                    return TestUtil.CreateRequirement(
                                        RequirementTypes.V1.Int32,
                                        isOptional: areOptional);
                                case 2:
                                    return TestUtil.CreateRequirement(
                                        RequirementTypes.V1.Int64,
                                        isOptional: areOptional);
                                case 3:
                                    return TestUtil.CreateRequirement(
                                        RequirementTypes.V1.SecureString,
                                        isOptional: areOptional);
                                case 4:
                                    return TestUtil.CreateRequirement(
                                        RequirementTypes.V1.String,
                                        isOptional: areOptional);
                                case 5:
                                    return TestUtil.CreateRequirement(
                                        RequirementTypes.V1.Uri,
                                        isOptional: areOptional);
                                case 6:
                                    return TestUtil.CreateRequirement(
                                        RequirementTypes.V1.GetFilePathType(
                                            new MockPathInteractor(
                                                y => y,
                                                y => true)),
                                        isOptional: areOptional);
                                default:
                                    throw new InvalidOperationException("Impossible test failure");
                            }
                        }
                        else
                        {
                            return TestUtil.CreateRequirement(
                                RequirementTypes.V1.String,
                                isOptional: areOptional);
                        }
                    })
                .ToArray();

        public static object GetDefaultValidObjectForRequirement(IRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (!TestUtil.defaultValidObjects.TryGetValue(requirement.Type, out object result))
            {
                if (requirement.Type.Type == typeof(FilePath) && requirement.Type.Version == new Version(1, 0, 0, 0))
                {
                    return new FilePath("Hello.txt", new MockPathInteractor(x => x, x => true));
                }
                else
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Unrecognized ConfigurationRequirementType '{0}'.",
                            requirement.Type.Type.ToString()));
                }
            }

            if (!requirement.CollectionInfo.HasValue)
            {
                return result;
            }
            else
            {
                object[] buffer = new object[
                    requirement.CollectionInfo.Value.MinimumCount.HasValue
                        ? requirement.CollectionInfo.Value.MinimumCount.Value
                        : 0];
                for (int counter = 0; counter < buffer.Length; counter++)
                {
                    buffer[counter] = result;
                }

                return buffer;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Test code.")]
        public static SecureString ToSecureString(this string source)
        {
            if (source == null)
            {
                return null;
            }

            SecureString result = new SecureString();
            foreach (char @char in source)
            {
                result.AppendChar(@char);
            }

            return result;
        }
    }
}
