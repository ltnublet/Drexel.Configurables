using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Tests.Mocks;
using Drexel.Configurables.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class BoundConfigurationTests
    {
        [TestMethod]
        public void BoundConfiguration_Ctor_Succeeds()
        {
            const int requirementCount = 50;

            IEnumerable<IConfigurationRequirement> requirements =
                TestUtil.CreateIConfigurationRequirementCollection(requirementCount, true);
            MockConfigurable configurable = new MockConfigurable(requirements);

            Dictionary<IConfigurationRequirement, object> validObjects =
                requirements
                    .Select(x =>
                        new KeyValuePair<IConfigurationRequirement, object>(
                            x,
                            TestUtil.GetDefaultValidObjectForRequirement(x)))
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value);

            BoundConfiguration boundConfiguration = new BoundConfiguration(configurable, validObjects);
        }
    }
}
