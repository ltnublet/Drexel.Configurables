using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;
using Drexel.Configurables.Tests.Common;
using Drexel.Configurables.Tests.Common.Mocks;
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
                (x, y, z) => null);

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
        public void ConfigurationRequirement_SecureString_Succeeds()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const long badInput = 8675309L;
            SecureString goodInput = "Hello world".ToSecureString();

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.SecureString,
                () => ConfigurationRequirement.SecureString(name, description),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_SecureString_PropagatesInfo()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const long badInput = 8675309L;
            SecureString[] goodInput =
                new SecureString[]
                {
                    "Hello world".ToSecureString()
                };
            CollectionInfo collectionInfo = new CollectionInfo(1, 4);

            IEnumerable<IConfigurationRequirement> dependsOn =
                TestUtil.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                TestUtil.CreateIConfigurationRequirementCollection(3);

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.SecureString,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.SecureString(
                    name,
                    description,
                    collectionInfo: collectionInfo,
                    dependsOn: dependsOn,
                    exclusiveWith: exclusiveWith),
                badInput,
                goodInput);
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
                TestUtil.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                TestUtil.CreateIConfigurationRequirementCollection(3);

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
        public void ConfigurationRequirement_Uri_Succeeds()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const long badInput = 8675309L;
            Uri goodInput = new Uri("https://www.example.com");

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.Uri,
                () => ConfigurationRequirement.Uri(name, description),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_Uri_PropagatesInfo()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const long badInput = 8675309L;
            Uri[] goodInput =
                new Uri[]
                {
                    new Uri("https://www.example.com")
                };
            CollectionInfo collectionInfo = new CollectionInfo(1, 4);

            IEnumerable<IConfigurationRequirement> dependsOn =
                TestUtil.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                TestUtil.CreateIConfigurationRequirementCollection(3);

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.Uri,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.Uri(
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
                TestUtil.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                TestUtil.CreateIConfigurationRequirementCollection(3);

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
                TestUtil.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                TestUtil.CreateIConfigurationRequirementCollection(3);

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
        public void ConfigurationRequirement_SimpleValidator_NullObject()
        {
            Assert.AreEqual(
                typeof(ArgumentNullException),
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        null,
                        null)
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_NotCollection_ValidObject()
        {
            Assert.IsNull(
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        null,
                        "Valid"));
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_NotCollection_InvalidObject()
        {
            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        null,
                        8675309L)
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_ValidObject()
        {
            Assert.IsNull(
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        new CollectionInfo(1),
                        new string[] { "Valid" }));
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_ObjectIsNotCollection()
        {
            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        new CollectionInfo(1),
                        "Invalid")
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_CollectionTooSmall()
        {
            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        new CollectionInfo(2),
                        new string[] { "Too small" })
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_CollectionIsRightSizeButContainsWrongType()
        {
            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        new CollectionInfo(1, 3),
                        new object[] { 8675309L, "Right type" })
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_CollectionTooLarge()
        {
            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        new CollectionInfo(1, 2),
                        new string[] { "Too big", "Too big", "Too big" })
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_CollectionIsEmpty()
        {
            Assert.IsNull(
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        new CollectionInfo(0, 5),
                        new object[0]));
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_AdditionalValidation_ReturnsNull()
        {
            Assert.IsNull(
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        null,
                        "Valid",
                        null,
                        (x, y, z) => null));
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_AdditionalValidation_ReturnsException()
        {
            Exception toReturn = new Exception("CONFIGURATIONREQUIREMENTTESTS_AdditionalValidation");

            Assert.AreEqual(
                toReturn.Message,
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        null,
                        "Valid",
                        null,
                        (x, y, z) => toReturn)
                    .Message);
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_AdditionalValidation_ThrowsException()
        {
            Exception toReturn = new Exception("CONFIGURATIONREQUIREMENTTESTS_AdditionalValidation");

            Assert.AreEqual(
                toReturn.Message,
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        null,
                        "Valid",
                        null,
                        (x, y, z) => throw toReturn)
                    .Message);
        }

        [TestMethod]
        public void ConfigurationRequirement_ToString_Succeeds()
        {
            ConfigurationRequirement requirement =
                TestUtil.CreateConfigurationRequirement();

            JsonCompare.Compare(requirement, requirement.ToString());
        }

        [TestMethod]
        public void ConfigurationRequirement_ToString_Collection_NoMaximumCount_Succeeds()
        {
            ConfigurationRequirement requirement =
                TestUtil.CreateConfigurationRequirement(collectionInfo: new CollectionInfo(3));

            JsonCompare.Compare(requirement, requirement.ToString());
        }

        [TestMethod]
        public void ConfigurationRequirement_ToString_Collection_MaximumCount_Succeeds()
        {
            ConfigurationRequirement requirement =
                TestUtil.CreateConfigurationRequirement(
                    collectionInfo: new CollectionInfo(3, 12));
            JsonCompare.Compare(requirement, requirement.ToString());
        }

        [TestMethod]
        public void ConfigurationRequirement_Validate_RaisesExceptionDuringValidation_Succeeds()
        {
            const string name = "DoNotCare";
            const string description = "DoNotCare";
            const bool isOptional = false;
            ConfigurationRequirementType type = ConfigurationRequirementType.String;
            const object validInput = null;

            ConfigurationRequirement requirement = new ConfigurationRequirement(
                name,
                description,
                type,
                isOptional,
                (x, y, z) => throw new NotImplementedException());

            Assert.AreEqual(typeof(NotImplementedException), requirement.Validate(validInput).GetType());
        }
    }
}
