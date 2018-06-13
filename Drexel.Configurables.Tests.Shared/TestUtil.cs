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
        private static Dictionary<ConfigurationRequirementType, object> defaultValidObjects =
            new Dictionary<ConfigurationRequirementType, object>()
            {
                [ConfigurationRequirementType.Bool] =
                    true,
                [ConfigurationRequirementType.FilePath] =
                    new FilePath("Hello.txt", new MockPathInteractor(x => x, x => true)),
                [ConfigurationRequirementType.Int32] =
                    8675309,
                [ConfigurationRequirementType.Int64] =
                    8675309L,
                [ConfigurationRequirementType.SecureString] =
                    "Hello world".ToSecureString(),
                [ConfigurationRequirementType.String] =
                    "Hello world",
                [ConfigurationRequirementType.Uri] =
                    new Uri("https://www.example.com/route/information?parameter1=value1&parameter2=value2")
            };

        private static Random random = new Random();

        private static long counter = 0;

        public static ConfigurationRequirement CreateConfigurationRequirement(
            string baseName = "ConfigurationRequirementName",
            string baseDescription = "Configuration requirement description.",
            ConfigurationRequirementType type = null,
            bool isOptional = false,
            Validator validator = null,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            return new ConfigurationRequirement(
                baseName + TestUtil.counter++,
                baseDescription + TestUtil.counter++,
                type ?? ConfigurationRequirementType.String,
                isOptional,
                validator ?? ((x, y, z) => null),
                collectionInfo,
                dependsOn ?? new IConfigurationRequirement[0],
                exclusiveWith ?? new IConfigurationRequirement[0]);
        }

        public static IEnumerable<IConfigurationRequirement> CreateIConfigurationRequirementCollection(
            int count,
            bool randomTypes = false) =>
            Enumerable
                .Range(0, count)
                .Select(x => TestUtil.CreateConfigurationRequirement(
                    type: randomTypes
                        ? ConfigurationRequirementType.Types[TestUtil.random.Next(0, ConfigurationRequirementType.Types.Count)]
                        : null))
                .ToArray();

        public static object GetDefaultValidObjectForRequirement(IConfigurationRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (!TestUtil.defaultValidObjects.TryGetValue(requirement.OfType, out object result))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unrecognized ConfigurationRequirementType '{0}'.",
                        requirement.OfType.Type.ToString()));
            }

            return result;
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
