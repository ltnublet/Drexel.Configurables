using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
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
    }
}
