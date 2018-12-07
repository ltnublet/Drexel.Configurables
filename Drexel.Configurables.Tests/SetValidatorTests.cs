using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class SetValidatorTests
    {
        [TestMethod]
        public void SetValidator_Ctor_Succeeds()
        {
            SetValidator<int> validator = new SetValidator<int>(
                Enumerable
                    .Range(0, 12)
                    .Select(x => new Contracts.SetRestrictionInfo<int>(x))
                    .ToArray());

            Assert.IsNotNull(validator);
        }
    }
}
