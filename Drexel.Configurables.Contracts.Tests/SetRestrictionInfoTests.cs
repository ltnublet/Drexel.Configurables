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

        [DataTestMethod]
        [DataRow(3, 5, true)]
        [DataRow(3, 3, false)]
        [DataRow(3, 1, false)]
        public void SetRestrictionInfo_IsAboveRange_Succeeds(int maximumTimesAllowed, int count, bool expected)
        {
            SetRestrictionInfo<int> info = new SetRestrictionInfo<int>(0, maximumTimesAllowed: maximumTimesAllowed);
            Assert.AreEqual(expected, info.IsAboveRange(count));
        }

        [DataTestMethod]
        [DataRow(3, 5, false)]
        [DataRow(3, 3, false)]
        [DataRow(3, 1, true)]
        public void SetRestrictionInfo_IsBelowRange_Succeeds(int minimumTimesAllowed, int count, bool expected)
        {
            SetRestrictionInfo<int> info = new SetRestrictionInfo<int>(0, minimumTimesAllowed: minimumTimesAllowed);
            Assert.AreEqual(expected, info.IsBelowRange(count));
        }
    }
}
