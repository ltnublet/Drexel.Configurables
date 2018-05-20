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

            Binding binding = new Binding(requirement, value);

            Assert.AreEqual(requirement, binding.Requirement);
            Assert.AreEqual(value, binding.Bound);
        }

        [TestMethod]
        public void Binding_Ctor_NullRequirement_ThrowsArgumentNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Binding(null, "NotNull"));
        }

        [TestMethod]
        public void Binding_Equals_Succeeds()
        {
            const string value = "Hello World";
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");

            Binding first = new Binding(requirement, value);
            Binding second = new Binding(requirement, value);

            Assert.IsTrue(first.Equals(second));
            Assert.IsTrue(second.Equals(first));
        }

        [TestMethod]
        public void Binding_Equals_Null_ReturnsFalse()
        {
            Binding binding = new Binding(ConfigurationRequirement.String("Name", "Description"), "Hello World");

            Assert.IsFalse(binding.Equals(null));
        }

        [TestMethod]
        public void Binding_Equals_NotBinding_ReturnsFalse()
        {
            Binding binding = new Binding(ConfigurationRequirement.String("Name", "Description"), "Hello World");

            Assert.IsFalse(binding.Equals("Hello world"));
        }

        [TestMethod]
        public void Binding_Equals_ValuesDoNotMatch_ReturnsFalse()
        {
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");
            Binding first = new Binding(requirement, "Hello World");
            Binding second = new Binding(requirement, "Goodbye World");

            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(second.Equals(first));
        }

        [TestMethod]
        public void Binding_Equals_OneHasValueNull_ReturnsFalse()
        {
            IConfigurationRequirement requirement = ConfigurationRequirement.String("Name", "Description");
            Binding first = new Binding(requirement, "Hello World");
            Binding second = new Binding(requirement, null);

            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(second.Equals(first));
        }

        [TestMethod]
        public void Binding_Equals_RequirementsDoNotMatch_ReturnsFalse()
        {
            const string value = "Hello World";
            IConfigurationRequirement requirement1 = ConfigurationRequirement.String("Name1", "Description");
            IConfigurationRequirement requirement2 = ConfigurationRequirement.String("Name2", "Description");

            Binding first = new Binding(requirement1, value);
            Binding second = new Binding(requirement2, value);

            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(second.Equals(first));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("Hello World")]
        public void Binding_GetHashCode_Success(object value)
        {
            Binding binding = new Binding(TestUtil.CreateConfigurationRequirement(), value);
            binding.GetHashCode();
        }

        [TestMethod]
        public void Binding_GetHashCode_DifferentInstances_ReturnsSameHash()
        {
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();
            object value = "Hello world";

            Binding first = new Binding(requirement, value);
            Binding second = new Binding(requirement, value);

            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }
    }
}
