using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void Configuration_Ctor_Succeeds()
        {
            IReadOnlyCollection<IRequirement> requirements = TestUtil.CreateRequirementCollection(
                10,
                randomTypes: true,
                areOptional: false);
            IReadOnlyDictionary<IRequirement, object> mappings = requirements.ToDictionary(
                x => x,
                x => TestUtil.GetDefaultValidObjectForRequirement(x));

            Configuration configuration = new Configuration(
                requirements,
                mappings);

            Assert.IsNotNull(configuration);

            CollectionAssert.AreEquivalent(requirements.ToArray(), configuration.Keys.ToArray());
            CollectionAssert.AreEquivalent(mappings.Values.ToArray(), configuration.Values.ToArray());

            foreach (KeyValuePair<IRequirement, object> mapping in mappings)
            {
                Assert.IsTrue(configuration.TryGetValue(mapping.Key, out object value));
                Assert.AreEqual(mapping.Value, value);
            }
        }

        [TestMethod]
        public void Configuration_Ctor_NullRequirements_ThrowsArgumentNull()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Configuration(null, new Dictionary<IRequirement, object>()));
        }

        [TestMethod]
        public void Configuration_Ctor_NullMappings_ThrowsArgumentNull()
        {
            IReadOnlyCollection<IRequirement> requirements = TestUtil.CreateRequirementCollection(3);
            Assert.IsNotNull(requirements);
            Assert.ThrowsException<ArgumentNullException>(
                () => new Configuration(requirements, null));
        }

        [TestMethod]
        public void Configuration_Ctor_MissingRequirements_ThrowsAggregate()
        {
            const int requirementCount = 12;
            const int expectedMissing = 2;

            IReadOnlyCollection<IRequirement> requirements = TestUtil.CreateRequirementCollection(requirementCount);
            Dictionary<IRequirement, object> mappings = requirements
                .Take(requirementCount - expectedMissing)
                .ToDictionary(
                    x => x,
                    x => TestUtil.GetDefaultValidObjectForRequirement(x));

            AggregateException exception = Assert.ThrowsException<AggregateException>(
                () => new Configuration(
                    requirements,
                    mappings));
        }
    }
}
