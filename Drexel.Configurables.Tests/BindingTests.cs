using System;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class BindingTests
    {
        [TestMethod]
        public void Binding_Ctor_Success()
        {
            const string value = "Valid";
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");

            Mapping binding = new Mapping(requirement, value);

            Assert.AreEqual(requirement, binding.Requirement);
            Assert.AreEqual(value, binding.Value);
        }

        [TestMethod]
        public void Binding_Ctor_NullRequirement_ThrowsArgumentNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Mapping(null, "NotNull"));
        }

        [TestMethod]
        public void Binding_Equals_Succeeds()
        {
            const string value = "Hello World";
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");

            Mapping first = new Mapping(requirement, value);
            Mapping second = new Mapping(requirement, value);

            Assert.IsTrue(first.Equals(second));
            Assert.IsTrue(second.Equals(first));
        }

        [TestMethod]
        public void Binding_Equals_Null_ReturnsFalse()
        {
            Mapping binding = new Mapping(ConfigurationRequirement.String("Name", "Description"), "Hello World");

            Assert.IsFalse(binding.Equals(null));
        }

        [TestMethod]
        public void Binding_Equals_NotBinding_ReturnsFalse()
        {
            Mapping binding = new Mapping(ConfigurationRequirement.String("Name", "Description"), "Hello World");

            Assert.IsFalse(binding.Equals("Hello world"));
        }

        [TestMethod]
        public void Binding_Equals_ValuesDoNotMatch_ReturnsFalse()
        {
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");
            Mapping first = new Mapping(requirement, "Hello World");
            Mapping second = new Mapping(requirement, "Goodbye World");

            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(second.Equals(first));
        }

        [TestMethod]
        public void Binding_Equals_OneHasValueNull_ReturnsFalse()
        {
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");
            Mapping first = new Mapping(requirement, "Hello World");
            Mapping second = new Mapping(requirement, null);

            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(second.Equals(first));
        }

        [TestMethod]
        public void Binding_Equals_RequirementsDoNotMatch_ReturnsFalse()
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
        public void Binding_GetHashCode_Success(object value)
        {
            Mapping binding = new Mapping(TestUtil.CreateConfigurationRequirement(), value);
            binding.GetHashCode();
        }

        [TestMethod]
        public void Binding_GetHashCode_DifferentInstances_ReturnsSameHash()
        {
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();
            object value = "Hello world";

            Mapping first = new Mapping(requirement, value);
            Mapping second = new Mapping(requirement, value);

            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }
    }
}
