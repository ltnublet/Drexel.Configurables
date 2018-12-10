using System;
using System.Linq;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class RequirementTests
    {
        [TestMethod]
        public void Requirement_Ctor_Succeeds()
        {
            Guid id = Guid.NewGuid();
            string name = "RequirementName";
            string description = "Requirement description.";
            IRequirementType<int> requirementType = RequirementTypes.V1.Int32;
            bool isOptional = false;
            CollectionInfo collectionInfo = new CollectionInfo(minimumCount: 3, maximumCount: 5);
            SetRestrictionInfo<int>[] restrictedToSet =
                new SetRestrictionInfo<int>[] { new SetRestrictionInfo<int>(3, 0, 5) };
            IRequirement[] dependsOn = Array.Empty<IRequirement>();
            IRequirement[] exclusiveWith = Array.Empty<IRequirement>();
            string validatorCallbackExceptionMessage = nameof(this.Requirement_Ctor_Succeeds);

            Requirement<int> requirement = new Requirement<int>(
                id: id,
                name: name,
                description: description,
                type: requirementType,
                isOptional: isOptional,
                collectionInfo: collectionInfo,
                restrictedToSet: restrictedToSet,
                dependsOn: dependsOn,
                exclusiveWith: exclusiveWith,
                (x, y) => new IntentionalTestException(validatorCallbackExceptionMessage));

            Assert.AreEqual(id, requirement.Id);
            Assert.AreEqual(name, requirement.Name);
            Assert.AreEqual(description, requirement.Description);
            Assert.AreEqual(requirementType, requirement.Type);
            Assert.AreEqual(isOptional, requirement.IsOptional);
            Assert.IsTrue(requirement.CollectionInfo.HasValue);
            Assert.AreEqual(collectionInfo, requirement.CollectionInfo.Value);
            CollectionAssert.AreEqual(restrictedToSet, requirement.RestrictedToSet.ToArray());
            CollectionAssert.AreEqual(dependsOn, requirement.DependsOn.ToArray());
            CollectionAssert.AreEqual(exclusiveWith, requirement.ExclusiveWith.ToArray());
            Assert.AreEqual(validatorCallbackExceptionMessage, requirement.Validate(0, null).Message);
        }

        [TestMethod]
        public void Requirement_Validate_Object_IsOfCorrectType_Succeeds()
        {
            Requirement<int> requirement = new Requirement<int>(
                Guid.NewGuid(),
                "RequirementName",
                "Requirement description.",
                RequirementTypes.V1.Int32,
                false,
                validationCallback: (x, y) => null);

            Assert.IsNull(requirement.Validate((object)0, null));
        }

        [TestMethod]
        public void Requirement_Validate_Object_IsOfCastableType_Succeeds()
        {
            Requirement<long> requirement = new Requirement<long>(
                Guid.NewGuid(),
                "RequirementName",
                "Requirement description.",
                RequirementTypes.V1.Int64,
                false,
                validationCallback: (x, y) => null);

            Assert.IsNull(requirement.Validate((short)0, null));
        }

        [TestMethod]
        public void Requirement_Validate_Object_IsOfIncorrectType_ThrowsInvalidCast()
        {
            Requirement<int> requirement = new Requirement<int>(
                Guid.NewGuid(),
                "RequirementName",
                "Requirement description.",
                RequirementTypes.V1.Int32,
                false,
                validationCallback: (x, y) => null);

            Exception exception = requirement.Validate("Not right type", null);
            Assert.AreEqual(typeof(InvalidCastException), exception.GetType());
        }
    }
}
