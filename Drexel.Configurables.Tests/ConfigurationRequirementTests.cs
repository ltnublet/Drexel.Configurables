using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;
using Drexel.Configurables.External.Shared.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class ConfigurationRequirementTests
    {
        [TestMethod]
        public void ConfigurationRequirement_Ctor_Succeeds()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const bool isOptional = false;

            ConfigurationRequirementType type = ConfigurationRequirementType.Int32;

            ConfigurationRequirement requirement = new ConfigurationRequirement(
                name,
                description,
                type,
                isOptional,
                (x, y) => null);

            Assert.AreEqual(name, requirement.Name);
            Assert.AreEqual(description, requirement.Description);
            Assert.AreEqual(type, requirement.OfType);
            Assert.AreEqual(isOptional, requirement.IsOptional);
            Assert.IsNull(requirement.CollectionInfo);
            Assert.IsFalse(requirement.DependsOn.Any());
            Assert.IsFalse(requirement.ExclusiveWith.Any());
        }

        [TestMethod]
        public void ConfigurationRequirement_Ctor_NullValidator_ThrowsArgumentNull()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const bool isOptional = false;

            ConfigurationRequirementType type = ConfigurationRequirementType.String;
            Validator validator = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                new ConfigurationRequirement(
                    name,
                    description,
                    type,
                    isOptional,
                    validator));
        }

        [TestMethod]
        public void ConfigurationRequirement_String_Succeeds()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const long badInput = 8675309L;
            const string goodInput = "Hello world";

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.String,
                () => ConfigurationRequirement.String(name, description),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_String_PropagatesInfo()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const long badInput = 8675309L;
            string[] goodInput =
                new string[]
                {
                    "Hello world"
                };
            CollectionInfo collectionInfo = new CollectionInfo(1, 4);

            IEnumerable<IConfigurationRequirement> dependsOn =
                ConfigurationRequirementTests.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                ConfigurationRequirementTests.CreateIConfigurationRequirementCollection(3);

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.String,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.String(
                    name,
                    description,
                    collectionInfo: collectionInfo,
                    dependsOn: dependsOn,
                    exclusiveWith: exclusiveWith),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_FilePath()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const string badInput = "Bad input";
            FilePath goodInput = new FilePath("DoesntMatter.txt", new MockPathInteractor(x => x, x => true));

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.FilePath,
                () => ConfigurationRequirement.FilePath(name, description),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_FilePath_PropagatesInfo()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const string badInput = "Bad input";
            FilePath[] goodInput =
                new FilePath[]
                {
                    new FilePath("DoesntMatter.txt", new MockPathInteractor(x => x, x => true))
                };
            CollectionInfo collectionInfo = new CollectionInfo(1, 4);

            IEnumerable<IConfigurationRequirement> dependsOn =
                ConfigurationRequirementTests.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                ConfigurationRequirementTests.CreateIConfigurationRequirementCollection(3);

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.FilePath,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.FilePath(
                    name,
                    description,
                    collectionInfo: collectionInfo,
                    dependsOn: dependsOn,
                    exclusiveWith: exclusiveWith),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_Int64_Success()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const string badInput = "Bad input";
            const long goodInput = 8675309L;

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.Int64,
                () => ConfigurationRequirement.Int64(name, description),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_Int64_PropagatesInfo()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const string badInput = "Bad input";
            long[] goodInput =
                new long[]
                {
                    8675309L
                };
            CollectionInfo collectionInfo = new CollectionInfo(1, 4);

            IEnumerable<IConfigurationRequirement> dependsOn =
                ConfigurationRequirementTests.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                ConfigurationRequirementTests.CreateIConfigurationRequirementCollection(3);

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.Int64,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.Int64(
                    name,
                    description,
                    collectionInfo: collectionInfo,
                    dependsOn: dependsOn,
                    exclusiveWith: exclusiveWith),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_ToString_Succeeds()
        {
            ConfigurationRequirement requirement =
                ConfigurationRequirementTests.CreateConfigurationRequirement();

            JsonCompare.Compare(requirement, requirement.ToString());
        }

        [TestMethod]
        public void ConfigurationRequirement_ToString_Collection_NoMaximumCount_Succeeds()
        {
            ConfigurationRequirement requirement =
                ConfigurationRequirementTests.CreateConfigurationRequirement(collectionInfo: new CollectionInfo(3));

            JsonCompare.Compare(requirement, requirement.ToString());
        }

        [TestMethod]
        public void ConfigurationRequirement_ToString_Collection_MaximumCount_Succeeds()
        {
            ConfigurationRequirement requirement =
                ConfigurationRequirementTests.CreateConfigurationRequirement(
                    collectionInfo: new CollectionInfo(3, 12));
            JsonCompare.Compare(requirement, requirement.ToString());
        }

        private static ConfigurationRequirement CreateConfigurationRequirement(
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
                baseName + Guid.NewGuid(),
                baseDescription + Guid.NewGuid(),
                type ?? ConfigurationRequirementType.String,
                isOptional,
                validator ?? ((x, y) => null),
                collectionInfo,
                dependsOn ?? new IConfigurationRequirement[0],
                exclusiveWith ?? new IConfigurationRequirement[0]);
        }

        private static IEnumerable<IConfigurationRequirement> CreateIConfigurationRequirementCollection(int count) =>
            Enumerable
                .Range(0, count)
                .Select(x => ConfigurationRequirementTests.CreateConfigurationRequirement())
                .ToArray();
    }
}
