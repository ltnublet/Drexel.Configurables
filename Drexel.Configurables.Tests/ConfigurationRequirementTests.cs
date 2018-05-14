using System;
using System.Linq;
using Drexel.Configurables.Contracts;
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
                x => null);

            Assert.AreEqual(name, requirement.Name);
            Assert.AreEqual(description, requirement.Description);
            Assert.AreEqual(type, requirement.OfType);
            Assert.AreEqual(isOptional, requirement.IsOptional);
            Assert.IsNull(requirement.CollectionInfo);
            Assert.IsFalse(requirement.DependsOn.Any());
            Assert.IsFalse(requirement.ExclusiveWith.Any());
        }

        [TestMethod]
        public void ConfigurationRequirement_ToString_Succeeds()
        {

        }
    }
}
