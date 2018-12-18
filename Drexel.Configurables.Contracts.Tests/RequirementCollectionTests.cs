using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Contracts.Tests
{
    [TestClass]
    public class RequirementCollectionTests
    {
        [TestMethod]
        public void RequirementCollection_Ctor_Succeeds()
        {
            RequirementCollection collection = new RequirementCollection(Array.Empty<IRequirement>());

            Assert.IsNotNull(collection);
            Assert.AreEqual(0, collection.Count);
        }
    }
}
