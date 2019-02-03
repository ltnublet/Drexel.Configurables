using System.Collections.Generic;
using Drexel.Configurables.Contracts.Relations;
using Drexel.Configurables.Contracts.Tests.Common.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Contracts.Tests.Relations
{
    [TestClass]
    public class RequirementRelationsBuilderTests
    {
        [DataTestMethod]
        [DataRow(
            RequirementRelation.ExclusiveWith,
            RequirementRelation.ExclusiveWith,
            RequirementRelation.ExclusiveWith)]
        [DataRow(
            RequirementRelation.DependsOn,
            RequirementRelation.DependsOn,
            RequirementRelation.DependedUpon)]
        public void RequirementRelationsBuilder_AddInternal_Succeeds(
            RequirementRelation addedRelation,
            RequirementRelation expectedFirst,
            RequirementRelation expectedSecond)
        {
            Dictionary<Requirement, Dictionary<Requirement, RequirementRelation>> buffer =
                new Dictionary<Requirement, Dictionary<Requirement, RequirementRelation>>();

            Requirement primary = new MockRequirement();
            Requirement secondary = new MockRequirement();

            RequirementRelationsBuilder.AddInternal(
                ref buffer,
                primary,
                secondary,
                addedRelation);

            Assert.AreEqual(2, buffer.Count);
            Assert.AreEqual(expectedFirst, buffer[primary][secondary]);
            Assert.AreEqual(expectedSecond, buffer[secondary][primary]);
        }
    }
}
