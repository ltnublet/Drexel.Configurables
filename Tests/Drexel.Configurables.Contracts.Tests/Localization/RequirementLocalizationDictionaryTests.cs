using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts.Localization;
using Drexel.Configurables.Contracts.Tests.Common.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Contracts.Tests.Localization
{
    [TestClass]
    public class RequirementLocalizationDictionaryTests
    {
        [TestMethod]
        public void RequirementLocalizationDictionary_Ctor()
        {
            Random random = new Random(8675309);

            IReadOnlyCollection<Requirement> requirements = Enumerable
                .Range(0, 10)
                .Select(x => new MockRequirement())
                .ToArray();
            IReadOnlyDictionary<Requirement, string> names = requirements
                .Where(x => random.Next(0, 2) == 0)
                .ToDictionary(x => x, x => Guid.NewGuid().ToString());
            IReadOnlyDictionary<Requirement, string> descriptions = requirements
                .Where(x => random.Next(0, 2) == 0)
                .ToDictionary(x => x, x => Guid.NewGuid().ToString());

            RequirementLocalizationDictionary dictionary = new RequirementLocalizationDictionary(
                requirements,
                names,
                descriptions);

            foreach (Requirement requirement in requirements)
            {
                RequirementLocalization localization = dictionary[requirement];

                Assert.AreEqual(
                    names.ContainsKey(requirement) ? names[requirement] : null,
                    localization.Name);
                Assert.AreEqual(
                    descriptions.ContainsKey(requirement) ? descriptions[requirement] : null,
                    localization.Description);
            }
        }
    }
}
