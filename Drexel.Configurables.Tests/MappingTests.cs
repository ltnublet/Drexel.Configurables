using System;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class MappingTests
    {
        [TestMethod]
        public void Mapping_Ctor_Success()
        {
            const string value = "Valid";
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");

            Mapping mapping = new Mapping(requirement, value);

            Assert.AreEqual(requirement, mapping.Key);
            Assert.AreEqual(value, mapping.Value);
        }

        [TestMethod]
        public void Mapping_Ctor_NullRequirement_ThrowsArgumentNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Mapping(null, "NotNull"));
        }

        [TestMethod]
        public void Mapping_Equals_Succeeds()
        {
            const string value = "Hello World";
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");

            Mapping first = new Mapping(requirement, value);
            Mapping second = new Mapping(requirement, value);

            Assert.IsTrue(first.Equals(second));
            Assert.IsTrue(second.Equals(first));
        }

        [TestMethod]
        public void Mapping_Equals_Null_ReturnsFalse()
        {
            Mapping mapping = new Mapping(ConfigurationRequirement.String("Name", "Description"), "Hello World");

            Assert.IsFalse(mapping.Equals(null));
        }

        [TestMethod]
        public void Mapping_Equals_NotMapping_ReturnsFalse()
        {
            Mapping mapping = new Mapping(ConfigurationRequirement.String("Name", "Description"), "Hello World");

            Assert.IsFalse(mapping.Equals("Hello world"));
        }

        [TestMethod]
        public void Mapping_Equals_ValuesDoNotMatch_ReturnsFalse()
        {
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");
            Mapping first = new Mapping(requirement, "Hello World");
            Mapping second = new Mapping(requirement, "Goodbye World");

            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(second.Equals(first));
        }

        [TestMethod]
        public void Mapping_Equals_OneHasValueNull_ReturnsFalse()
        {
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");
            Mapping first = new Mapping(requirement, "Hello World");
            Mapping second = new Mapping(requirement, null);

            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(second.Equals(first));
        }

        [TestMethod]
        public void Mapping_Equals_RequirementsDoNotMatch_ReturnsFalse()
        {
            const string value = "Hello World";
            IConfigurationRequirement requirement1 = ConfigurationRequirement.String("Name1", "Description");
            IConfigurationRequirement requirement2 = ConfigurationRequirement.String("Name2", "Description");

            Mapping first = new Mapping(requirement1, value);
            Mapping second = new Mapping(requirement2, value);

            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(second.Equals(first));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("Hello World")]
        public void Mapping_GetHashCode_Success(object value)
        {
            Mapping mapping = new Mapping(TestUtil.CreateConfigurationRequirement(), value);
            mapping.GetHashCode();
        }

        [TestMethod]
        public void Mapping_GetHashCode_DifferentInstances_ReturnsSameHash()
        {
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();
            object value = "Hello world";

            Mapping first = new Mapping(requirement, value);
            Mapping second = new Mapping(requirement, value);

            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }
    }
}
