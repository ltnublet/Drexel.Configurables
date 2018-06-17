using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;
using Drexel.Configurables.Tests.Common;
using Drexel.Configurables.Tests.Common.Mocks;
using Drexel.Configurables.Tests.Mocks;
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
            const bool isOptional = true;
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
                isOptional,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.SecureString(
                    name,
                    description,
                    isOptional,
                    collectionInfo,
                    dependsOn,
                    exclusiveWith),
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
            const bool isOptional = true;
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
                isOptional,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.String(
                    name,
                    description,
                    isOptional,
                    collectionInfo,
                    dependsOn,
                    exclusiveWith),
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
            const bool isOptional = true;
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
                isOptional,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.Uri(
                    name,
                    description,
                    isOptional,
                    collectionInfo,
                    dependsOn,
                    exclusiveWith),
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
            const bool isOptional = true;
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
                isOptional,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.FilePath(
                    name,
                    description,
                    isOptional,
                    collectionInfo,
                    dependsOn,
                    exclusiveWith),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_Bool_Success()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const string badInput = "Bad input";
            const bool goodInput = true;

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.Bool,
                () => ConfigurationRequirement.Bool(name, description),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_Bool_PropagatesInfo()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const bool isOptional = true;
            const string badInput = "Bad input";
            bool[] goodInput =
                new bool[]
                {
                    true,
                    false,
                    true
                };
            CollectionInfo collectionInfo = new CollectionInfo(1, 4);

            IEnumerable<IConfigurationRequirement> dependsOn =
                TestUtil.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                TestUtil.CreateIConfigurationRequirementCollection(3);

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.Bool,
                isOptional,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.Bool(
                    name,
                    description,
                    isOptional,
                    collectionInfo,
                    dependsOn,
                    exclusiveWith),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_Int32_Success()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const string badInput = "Bad input";
            const int goodInput = 8675309;

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.Int32,
                () => ConfigurationRequirement.Int32(name, description),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_Int32_PropagatesInfo()
        {
            const string name = "ConfigurationRequirementName";
            const string description = "Configuration requirement description.";
            const bool isOptional = true;
            const string badInput = "Bad input";
            int[] goodInput =
                new int[]
                {
                    8675309
                };
            CollectionInfo collectionInfo = new CollectionInfo(1, 4);

            IEnumerable<IConfigurationRequirement> dependsOn =
                TestUtil.CreateIConfigurationRequirementCollection(3);
            IEnumerable<IConfigurationRequirement> exclusiveWith =
                TestUtil.CreateIConfigurationRequirementCollection(3);

            AssertUtil.Compare(
                name,
                description,
                ConfigurationRequirementType.Int32,
                isOptional,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.Int32(
                    name,
                    description,
                    isOptional,
                    collectionInfo,
                    dependsOn,
                    exclusiveWith),
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
            const bool isOptional = true;
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
                isOptional,
                collectionInfo,
                dependsOn,
                exclusiveWith,
                () => ConfigurationRequirement.Int64(
                    name,
                    description,
                    isOptional,
                    collectionInfo,
                    dependsOn,
                    exclusiveWith),
                badInput,
                goodInput);
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_NullObject()
        {
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();

            Assert.AreEqual(
                typeof(ArgumentNullException),
                ConfigurationRequirement
                    .SimpleValidator(
                        requirement.OfType,
                        null,
                        requirement,
                        new MockConfiguration())
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_NotCollection_ValidObject()
        {
            const string value = "Valid";
            ConfigurationRequirementType type = ConfigurationRequirementType.String;

            Assert.IsNull(
                ConfigurationRequirement
                    .SimpleValidator(
                        type,
                        value,
                        TestUtil.CreateConfigurationRequirement(type: type),
                        new MockConfiguration()));
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_NotCollection_InvalidObject()
        {
            const long value = 8675309L;
            ConfigurationRequirementType type = ConfigurationRequirementType.String;

            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        type,
                        value,
                        TestUtil.CreateConfigurationRequirement(),
                        new MockConfiguration())
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_ValidObject()
        {
            IConfigurationRequirement requirement =
                TestUtil.CreateConfigurationRequirement(collectionInfo: new CollectionInfo(1));
            object value = TestUtil.GetDefaultValidObjectForRequirement(requirement);

            Assert.IsNull(
                ConfigurationRequirement
                    .SimpleValidator(
                        requirement.OfType,
                        value,
                        requirement,
                        new MockConfiguration()));
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_ObjectIsNotCollection()
        {
            const string value = "Invalid";
            ConfigurationRequirementType type = ConfigurationRequirementType.String;
            IConfigurationRequirement requirement =
                TestUtil.CreateConfigurationRequirement(
                    type: type,
                    collectionInfo: new CollectionInfo(1));

            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        type,
                        value,
                        requirement,
                        new MockConfiguration())
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_CollectionTooSmall()
        {
            string[] value = new string[] { "Too small" };
            ConfigurationRequirementType type = ConfigurationRequirementType.String;
            IConfigurationRequirement requirement =
                TestUtil.CreateConfigurationRequirement(
                    type: type,
                    collectionInfo: new CollectionInfo(2));

            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        ConfigurationRequirementType.String,
                        value,
                        requirement,
                        new MockConfiguration())
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_CollectionIsRightSizeButContainsWrongType()
        {
            object[] value = new object[] { 8675309L, 123456 };
            ConfigurationRequirementType type = ConfigurationRequirementType.Int64;
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement(
                type: type,
                collectionInfo: new CollectionInfo(1, 3));

            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        type,
                        value,
                        requirement,
                        new MockConfiguration())
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_CollectionTooLarge()
        {
            string[] value = new string[] { "Too big", "Too big", "Too big" };
            ConfigurationRequirementType type = ConfigurationRequirementType.String;
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement(
                type: type,
                collectionInfo: new CollectionInfo(1, 2));

            Assert.AreEqual(
                typeof(ArgumentException),
                ConfigurationRequirement
                    .SimpleValidator(
                        type,
                        value,
                        requirement,
                        new MockConfiguration())
                    ?.GetType());
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_Collection_CollectionIsEmpty()
        {
            object[] value = new object[0];
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();

            Assert.IsNull(
                ConfigurationRequirement
                    .SimpleValidator(
                        requirement.OfType,
                        value,
                        requirement,
                        new MockConfiguration()));
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_AdditionalValidation_ReturnsNull()
        {
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();

            Assert.IsNull(
                ConfigurationRequirement
                    .SimpleValidator(
                        requirement.OfType,
                        TestUtil.GetDefaultValidObjectForRequirement(requirement),
                        requirement,
                        new MockConfiguration(),
                        (x, y, z) => null));
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_AdditionalValidation_ReturnsException()
        {
            Exception toReturn = new Exception("CONFIGURATIONREQUIREMENTTESTS_AdditionalValidation");
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();

            Assert.AreEqual(
                toReturn.Message,
                ConfigurationRequirement
                    .SimpleValidator(
                        requirement.OfType,
                        TestUtil.GetDefaultValidObjectForRequirement(requirement),
                        requirement,
                        new MockConfiguration(),
                        (x, y, z) => toReturn)
                    .Message);
        }

        [TestMethod]
        public void ConfigurationRequirement_SimpleValidator_AdditionalValidation_ThrowsException()
        {
            Exception toReturn = new Exception("CONFIGURATIONREQUIREMENTTESTS_AdditionalValidation");
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();

            Assert.AreEqual(
                toReturn.Message,
                ConfigurationRequirement
                    .SimpleValidator(
                        requirement.OfType,
                        TestUtil.GetDefaultValidObjectForRequirement(requirement),
                        requirement,
                        new MockConfiguration(),
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
