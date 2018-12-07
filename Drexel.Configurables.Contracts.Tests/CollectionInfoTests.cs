using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Contracts.Tests
{
    [TestClass]
    public class CollectionInfoTests
    {
        public static IEnumerable<object[]> EqualsTestCases { get; } =
            new object[][]
            {
                new object[]
                {
                    null,
                    null,
                    null,
                    null,
                    true
                },
                new object[]
                {
                    null,
                    3,
                    null,
                    3,
                    true
                },
                new object[]
                {
                    3,
                    null,
                    3,
                    null,
                    true
                },
                new object[]
                {
                    2,
                    5,
                    2,
                    5,
                    true
                },
                new object[]
                {
                    null,
                    null,
                    3,
                    6,
                    false
                },
                new object[]
                {
                    3,
                    6,
                    null,
                    null,
                    false
                },
                new object[]
                {
                    null,
                    5,
                    null,
                    3,
                    false
                }
            };

        [TestMethod]
        public void CollectionInfo_Ctor_Parameterless_Succeeds()
        {
            CollectionInfo info = new CollectionInfo();

            Assert.AreEqual(null, info.MaximumCount);
            Assert.AreEqual(null, info.MinimumCount);
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(0, 1)]
        [DataRow(2, 4)]
        [DataRow(3, 3)]
        public void CollectionInfo_Ctor_Parameters_Succeeds(int? minimumCount, int? maximumCount)
        {
            CollectionInfo info = new CollectionInfo(
                minimumCount: minimumCount,
                maximumCount: maximumCount);

            Assert.AreEqual(minimumCount, info.MinimumCount);
            Assert.AreEqual(maximumCount, info.MaximumCount);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        [DataRow(int.MinValue)]
        public void CollectionInfo_Ctor_IllegalMinimumCount_ThrowsArgumentOutOfRange(int illegalMinimumCount)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new CollectionInfo(minimumCount: illegalMinimumCount));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void CollectionInfo_Ctor_IllegalMaximumCount_ThrowsArgumentOutOfRange(int illegalMaximumCount)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new CollectionInfo(maximumCount: illegalMaximumCount));
        }

        [DataTestMethod]
        [DataRow(8, 7)]
        public void CollectionInfo_Ctor_IllegalMinimumMaximumCombination_ThrowsArgumentOutOfRange(
            int minimumCount,
            int maximumCount)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new CollectionInfo(
                    minimumCount: minimumCount,
                    maximumCount: maximumCount));
        }

        [DataTestMethod]
        [DynamicData(nameof(EqualsTestCases))]
        public void CollectionInfo_IEquatable_Equals_Succeeds(
            int? leftMinimumCount,
            int? leftMaximumCount,
            int? rightMinimumCount,
            int? rightMaximumCount,
            bool areEqual)
        {
            CollectionInfo left = new CollectionInfo(leftMinimumCount, leftMaximumCount);
            CollectionInfo right = new CollectionInfo(rightMinimumCount, rightMaximumCount);

            bool leftEqualsRight = left.Equals(right);
            bool rightEqualsLeft = right.Equals(left);
            if (areEqual)
            {
                Assert.IsTrue(leftEqualsRight);
                Assert.IsTrue(rightEqualsLeft);
            }
            else
            {
                Assert.IsFalse(leftEqualsRight);
                Assert.IsFalse(rightEqualsLeft);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(EqualsTestCases))]
        public void CollectionInfo_EqualsOperator_Succeeds(
            int? leftMinimumCount,
            int? leftMaximumCount,
            int? rightMinimumCount,
            int? rightMaximumCount,
            bool areEqual)
        {
            CollectionInfo left = new CollectionInfo(leftMinimumCount, leftMaximumCount);
            CollectionInfo right = new CollectionInfo(rightMinimumCount, rightMaximumCount);

            bool leftEqualsRight = left == right;
            bool rightEqualsLeft = right == left;
            if (areEqual)
            {
                Assert.IsTrue(leftEqualsRight);
                Assert.IsTrue(rightEqualsLeft);
            }
            else
            {
                Assert.IsFalse(leftEqualsRight);
                Assert.IsFalse(rightEqualsLeft);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(EqualsTestCases))]
        public void CollectionInfo_NotEqualsOperator_Succeeds(
            int? leftMinimumCount,
            int? leftMaximumCount,
            int? rightMinimumCount,
            int? rightMaximumCount,
            bool areEqual)
        {
            CollectionInfo left = new CollectionInfo(leftMinimumCount, leftMaximumCount);
            CollectionInfo right = new CollectionInfo(rightMinimumCount, rightMaximumCount);

            bool leftDoesNotEqualRight = left != right;
            bool rightDoesNotEqualLeft = right != left;
            if (areEqual)
            {
                Assert.IsFalse(leftDoesNotEqualRight);
                Assert.IsFalse(rightDoesNotEqualLeft);
            }
            else
            {
                Assert.IsTrue(leftDoesNotEqualRight);
                Assert.IsTrue(rightDoesNotEqualLeft);
            }
        }

        [TestMethod]
        public void CollectionInfo_Equals_AreCollectionInfo_Succeeds()
        {
            CollectionInfo left = new CollectionInfo(3, 12);
            CollectionInfo right = new CollectionInfo(3, 12);

            Assert.IsTrue(left.Equals((object)right));
            Assert.IsTrue(((object)left).Equals(right));
            Assert.IsTrue(right.Equals((object)left));
            Assert.IsTrue(((object)right).Equals(left));
        }

        [DataTestMethod]
        [DataRow(12)]
        [DataRow("hello")]
        [DataRow(new float[] { 1F, 2F, 3F })]
        public void CollectionInfo_Equals_AreNotCollectionInfo_Succeeds(object notCollectionInfo)
        {
            CollectionInfo isCollectionInfo = new CollectionInfo(3, 8);
            Assert.IsFalse(isCollectionInfo.Equals(notCollectionInfo));
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(3, null)]
        [DataRow(null, 3)]
        [DataRow(5, 12)]
        public void CollectionInfo_GetHashCode_AreSame_GetSameHash(int? minimumCount, int? maximumCount)
        {
            CollectionInfo first = new CollectionInfo(minimumCount, maximumCount);
            CollectionInfo second = new CollectionInfo(minimumCount, maximumCount);

            Assert.IsTrue(first.GetHashCode() == second.GetHashCode());
        }

        [DataTestMethod]
        [DataRow(3, 5, 7, 12)]
        [DataRow(3, 4, 3, 5)]
        [DataRow(null, 6, null, 8)]
        [DataRow(null, null, 2, 5)]
        [DataRow(null, 3, 3, null)]
        public void CollectionInfo_GetHashCode_AreDifferent_GetDifferentHashes(
            int? firstMinimumCount,
            int? firstMaximumCount,
            int? secondMinimumCount,
            int? secondMaximumCount)
        {
            CollectionInfo first = new CollectionInfo(firstMinimumCount, firstMaximumCount);
            CollectionInfo second = new CollectionInfo(secondMinimumCount, secondMaximumCount);

            Assert.IsTrue(first.GetHashCode() != second.GetHashCode());
        }
    }
}
