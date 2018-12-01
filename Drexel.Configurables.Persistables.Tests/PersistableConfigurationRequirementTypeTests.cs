using System;
using System.Collections.Generic;
using System.Text;
using Drexel.Configurables.Persistables.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Persistables.Tests
{
    [TestClass]
    public class PersistableConfigurationRequirementTypeTests
    {
        [DataTestMethod]
        [DataRow(true, "true")]
        [DataRow(false, "false")]
        public void PersistableConfigurationRequirementType_V1_BoolEncode_Succeeds(
            bool input,
            string expected)
        {
            Assert.AreEqual(
                expected,
                PersistableConfigurationRequirementType.V1.BoolEncode(input));
        }

        public void PersistableConfigurationRequirementType_V1_BoolEncode_
    }
}
