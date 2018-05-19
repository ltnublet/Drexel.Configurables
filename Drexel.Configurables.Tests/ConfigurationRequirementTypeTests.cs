using System.Linq;
using Drexel.Configurables.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class ConfigurationRequirementTypeTests
    {
        [TestMethod]
        [Description("Because the Types property is generated using reflection, make sure it behaves as expected.")]
        public void ConfigurationRequirementType_Static_Types_Succeeds()
        {
            CollectionAssert.AreEquivalent(
                new ConfigurationRequirementType[]
                {
                    ConfigurationRequirementType.Bool,
                    ConfigurationRequirementType.FilePath,
                    ConfigurationRequirementType.Int32,
                    ConfigurationRequirementType.Int64,
                    ConfigurationRequirementType.SecureString,
                    ConfigurationRequirementType.String,
                    ConfigurationRequirementType.Uri
                },
                ConfigurationRequirementType.Types.ToArray());
        }

        [TestMethod]
        public void ConfigurationRequirementType_Equals_AreEqual_ReturnsTrue()
        {
            ConfigurationRequirementType first = ConfigurationRequirementType.String;
            ConfigurationRequirementType second = new ConfigurationRequirementType(typeof(string));

            Assert.IsTrue(first.Equals(second));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(typeof(string))]
        [DataRow(8675309)]
        public void ConfigurationRequirementType_Equals_NotEqual_ReturnsFalse(object notEqual)
        {
            Assert.IsFalse(ConfigurationRequirementType.String.Equals(notEqual));
        }

        [TestMethod]
        public void ConfigurationRequirementType_GetHashCode_AreEqual()
        {
            Assert.AreEqual(
                ConfigurationRequirementType.String.GetHashCode(),
                new ConfigurationRequirementType(typeof(string)).GetHashCode());
        }
    }
}
