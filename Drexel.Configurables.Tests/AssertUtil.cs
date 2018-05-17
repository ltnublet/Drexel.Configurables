using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    public class AssertUtil
    {
        public static void Compare(
            string expectedName,
            string expectedDescription,
            ConfigurationRequirementType expectedType,
            Func<IConfigurationRequirement> requirementFactory,
            object badInput,
            object goodInput)
        {
            IConfigurationRequirement requirement = requirementFactory.Invoke();

            Assert.AreEqual(expectedName, requirement.Name);
            Assert.AreEqual(expectedDescription, requirement.Description);
            Assert.AreEqual(expectedType, requirement.OfType);
            Assert.IsNull(requirement.CollectionInfo);
            Assert.IsFalse(requirement.DependsOn.Any());
            Assert.IsFalse(requirement.ExclusiveWith.Any());

            Assert.AreEqual(typeof(ArgumentNullException), requirement.Validate(null).GetType());
            Assert.AreEqual(typeof(ArgumentException), requirement.Validate(badInput).GetType());
            Assert.IsNull(requirement.Validate(goodInput));
        }

        public static void Compare(
            string expectedName,
            string expectedDescription,
            ConfigurationRequirementType expectedType,
            CollectionInfo collectionInfo,
            IEnumerable<IConfigurationRequirement> dependsOn,
            IEnumerable<IConfigurationRequirement> exclusiveWith,
            Func<IConfigurationRequirement> requirementFactory,
            object badInput,
            object goodInput)
        {
            IConfigurationRequirement requirement = requirementFactory.Invoke();

            Assert.AreEqual(expectedName, requirement.Name);
            Assert.AreEqual(expectedDescription, requirement.Description);
            Assert.AreEqual(expectedType, requirement.OfType);
            Assert.AreEqual(collectionInfo, requirement.CollectionInfo);
            CollectionAssert.AreEquivalent(dependsOn.ToArray(), requirement.DependsOn.ToArray());
            CollectionAssert.AreEquivalent(exclusiveWith.ToArray(), requirement.ExclusiveWith.ToArray());
            
            Assert.AreEqual(typeof(ArgumentNullException), requirement.Validate(null).GetType());
            Assert.AreEqual(typeof(ArgumentException), requirement.Validate(badInput).GetType());
            Assert.IsNull(requirement.Validate(goodInput));
        }
    }
}
