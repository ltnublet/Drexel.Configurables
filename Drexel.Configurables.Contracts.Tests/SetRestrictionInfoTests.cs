using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Contracts.Tests
{
    [TestClass]
    public class SetRestrictionInfoTests
    {
        [TestMethod]
        public void SetRestrictionInfo_Ctor_Succeeds()
        {
            const string value = "Foo";

            SetRestrictionInfo<string> info = new SetRestrictionInfo<string>(value);

            Assert.AreEqual(value, info.Value);
            Assert.AreEqual(null, info.MaximumTimesAllowed);
            Assert.AreEqual(null, info.MinimumTimesAllowed);
        }

        [TestMethod]
        public void SetRestrictionInfo_Ctor_LegalMinimumTimesAllowed()
        {
            const string value = "Foo";
            const int minimumTimesAllowed = 3;

            SetRestrictionInfo<string> info = new SetRestrictionInfo<string>(
                value,
                minimumTimesAllowed: minimumTimesAllowed);

            Assert.AreEqual(value, info.Value);
            Assert.AreEqual(null, info.MaximumTimesAllowed);
            Assert.AreEqual(minimumTimesAllowed, info.MinimumTimesAllowed);
        }

        [TestMethod]
        public void SetRestrictionInfo_Ctor_LegalMaximumTimesAllowed()
        {
            const string value = "Foo";
            const int maximumTimesAllowed = 12;

            SetRestrictionInfo<string> info = new SetRestrictionInfo<string>(
                value,
                maximumTimesAllowed: maximumTimesAllowed);

            Assert.AreEqual(value, info.Value);
            Assert.AreEqual(maximumTimesAllowed, info.MaximumTimesAllowed);
            Assert.AreEqual(null, info.MinimumTimesAllowed);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        [DataRow(int.MinValue)]
        public void SetRestrictionInfo_Ctor_IllegalMinimumTimesAllowed_ThrowsArgumentOutOfRange(
            int illegalMinimumTimes)
        {
            const string value = "Foo";
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () =>
                new SetRestrictionInfo<string>(
                    value,
                    minimumTimesAllowed: illegalMinimumTimes));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void SetRestrictionInfo_Ctor_IllegalMaximumTimesAlloweed_ThrowsArgumentOutOfRange(
            int illegalMaximumTimes)
        {
            const string value = "Foo";
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () =>
                new SetRestrictionInfo<string>(
                    value,
                    maximumTimesAllowed: illegalMaximumTimes));
        }

        [TestMethod]
        public void SetRestrictionInfo_Ctor_MaximumTimesLessThanMinimumTimes_ThrowsArgumentOutOfRange()
        {
            const string value = "Foo";
            const int minimumTimesAllowed = 12;
            const int maximumTimesAllowed = minimumTimesAllowed - 1;

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () =>
                new SetRestrictionInfo<string>(
                    value,
                    minimumTimesAllowed: minimumTimesAllowed,
                    maximumTimesAllowed: maximumTimesAllowed));
        }
    }
}
