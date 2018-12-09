using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using INonGenericEnumerable = System.Collections.IEnumerable;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class SetValidatorTests
    {
        [TestMethod]
        public void SetValidator_Ctor_NoParameters_Succeeds()
        {
            SetValidator<int> validator = new SetValidator<int>();
            Assert.IsNotNull(validator);
        }

        [TestMethod]
        public void SetValidator_Ctor_ValidSet_Succeeds()
        {
            SetValidator<int> validator = new SetValidator<int>(
                new SetRestrictionInfo<int>[]
                {
                    new SetRestrictionInfo<int>(0),
                    new SetRestrictionInfo<int>(1),
                    new SetRestrictionInfo<int>(2)
                });

            Assert.IsNotNull(validator);
        }

        [TestMethod]
        public void SetValidator_Ctor_SetContainsDuplicateValues_ThrowsArgument()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new SetValidator<int>(
                    new SetRestrictionInfo<int>[]
                    {
                        new SetRestrictionInfo<int>(1),
                        new SetRestrictionInfo<int>(2),
                        new SetRestrictionInfo<int>(2),
                        new SetRestrictionInfo<int>(3)
                    }));
        }

        [TestMethod]
        public void SetValidator_Validate_Succeeds()
        {
            SetValidator<int> validator = new SetValidator<int>();

            IEnumerable<int> asGeneric = new int[] { 0 };
            INonGenericEnumerable asNonGeneric = asGeneric;
            validator.Validate(asGeneric);
            validator.Validate(asNonGeneric);
        }

        [TestMethod]
        public void SetValidator_Validate_WrongType_NoRestrictedSet_Succeeds()
        {
            SetValidator<int> validator = new SetValidator<int>();
            validator.Validate(
                new string[]
                {
                    "I am the wrong type!"
                });
        }

        [TestMethod]
        public void SetValidator_Validate_WrongType_HasRestrictedSet_ThrowsValueOfWrongType()
        {
            const string value = "I am the wrong type!";
            SetValidator<int> validator = new SetValidator<int>(
                new SetRestrictionInfo<int>[]
                {
                    new SetRestrictionInfo<int>(10)
                });

            ValueOfWrongTypeException exception = Assert.ThrowsException<ValueOfWrongTypeException>(
                () =>
                validator.Validate(
                    new string[]
                    {
                        value
                    }));

            Assert.AreEqual(value, exception.Value);
        }

        [TestMethod]
        public void SetValidator_Validate_NullSet_MinimumCountNotMet_ThrowsMinimumCountNotMet()
        {
            const int minimumCount = 3;
            const int actualCount = 2;

            SetValidator<int> validator = new SetValidator<int>(
                collectionInfo: new CollectionInfo(minimumCount: 3));

            MinimumCountNotMetException exception = Assert.ThrowsException<MinimumCountNotMetException>(
                () => validator.Validate(new int[] { 1, 2 }));

            Assert.AreEqual(minimumCount, exception.MinimumCount);
            Assert.AreEqual(actualCount, exception.ActualCount);
        }

        [TestMethod]
        public void SetValidator_Validate_NullSet_MaximumCountExceeded_ThrowsMaximumCountExceeded()
        {
            const int maximumCount = 3;
            const int actualCount = 5;

            SetValidator<int> validator = new SetValidator<int>(
                collectionInfo: new CollectionInfo(maximumCount: maximumCount));

            MaximumCountExceededException exception = Assert.ThrowsException<MaximumCountExceededException>(
                () => validator.Validate(Enumerable.Range(0, actualCount)));

            Assert.AreEqual(maximumCount, exception.MaximumCount);
            Assert.AreEqual(actualCount, exception.ActualCount);
        }

        [TestMethod]
        public void SetValidator_Validate_MinimumCountNotMet_ThrowsMinimumCountNotMet()
        {
            const int value = 0;
            const int minimumCount = 3;
            const int actualCount = 1;

            SetValidator<int> validator = new SetValidator<int>(
                restrictedToSet: new SetRestrictionInfo<int>[]
                {
                    new SetRestrictionInfo<int>(value)
                },
                collectionInfo: new CollectionInfo(minimumCount: minimumCount));

            MinimumCountNotMetException exception = Assert.ThrowsException<MinimumCountNotMetException>(
                () => validator.Validate(Enumerable.Range(0, actualCount).Select(x => value).ToArray()));

            Assert.AreEqual(minimumCount, exception.MinimumCount);
            Assert.AreEqual(actualCount, exception.ActualCount);
        }

        [TestMethod]
        public void SetValidator_Validate_MaximumCountExceeded_ThrowsMaximumCountExceeded()
        {
            const int value = 0;
            const int maximumCount = 3;
            const int actualCount = 5;

            SetValidator<int> validator = new SetValidator<int>(
                restrictedToSet: new SetRestrictionInfo<int>[]
                {
                    new SetRestrictionInfo<int>(value)
                },
                collectionInfo: new CollectionInfo(maximumCount: maximumCount));

            MaximumCountExceededException exception = Assert.ThrowsException<MaximumCountExceededException>(
                () => validator.Validate(Enumerable.Range(0, actualCount).Select(x => value).ToArray()));

            Assert.AreEqual(maximumCount, exception.MaximumCount);
            Assert.AreEqual(actualCount, exception.ActualCount);
        }

        [TestMethod]
        public void SetValidator_Validate_NullSet_ThrowsArgumentNull()
        {
            SetValidator<int> validator = new SetValidator<int>();
            Assert.ThrowsException<ArgumentNullException>(
                () => validator.Validate((INonGenericEnumerable)null));
            Assert.ThrowsException<ArgumentNullException>(
                () => validator.Validate((IEnumerable<int>)null));
        }

        [TestMethod]
        public void SetValidator_Validate_NonGeneric_NullSet_MaximumCountExceeded_ThrowsMaximumCountExceeded()
        {
            const int maximumCount = 3;
            const int actualCount = 5;

            SetValidator<int> validator = new SetValidator<int>(
                collectionInfo: new CollectionInfo(maximumCount: maximumCount));
            MaximumCountExceededException exception = Assert.ThrowsException<MaximumCountExceededException>(
                () => validator.Validate((INonGenericEnumerable)Enumerable.Range(0, actualCount)));

            Assert.AreEqual(actualCount, exception.ActualCount);
            Assert.AreEqual(maximumCount, exception.MaximumCount);
        }

        [TestMethod]
        public void SetValidator_Validate_NonGeneric_NullSet_MinimumCountNotMet_ThrowsMinimimCountNotMet()
        {
            const int minimumCount = 3;
            const int actualCount = 2;

            SetValidator<int> validator = new SetValidator<int>(
                collectionInfo: new CollectionInfo(minimumCount: minimumCount));
            MinimumCountNotMetException exception = Assert.ThrowsException<MinimumCountNotMetException>(
                () => validator.Validate((INonGenericEnumerable)Enumerable.Range(0, actualCount)));

            Assert.AreEqual(actualCount, exception.ActualCount);
            Assert.AreEqual(minimumCount, exception.MinimumCount);
        }

        [TestMethod]
        public void SetValidator_Validate_ValueNotInSet_ThrowsValueNotInSet()
        {
            int[] allowedValues = new int[] { 0, 1, 2, 3 };
            int notInSet = 5;

            SetValidator<int> validator = new SetValidator<int>(
                allowedValues.Select(x => new SetRestrictionInfo<int>(x)).ToArray());

            ValueNotInSetException exception = Assert.ThrowsException<ValueNotInSetException>(
                () => validator.Validate(allowedValues.Concat(new int[] { notInSet })));

            Assert.AreEqual(notInSet, exception.Value);
        }

        [TestMethod]
        public void SetValidator_Validate_ValueBelowMinimumTimesThreshold_ThrowsValueBelowMinimumTimesThreshold()
        {
            const int value = 5;
            const int minimumCount = 3;
            const int expectedTimesSeen = minimumCount - 1;

            SetValidator<int> validator = new SetValidator<int>(
                new SetRestrictionInfo<int>[]
                {
                    new SetRestrictionInfo<int>(value, minimumTimesAllowed: minimumCount)
                });

            ValueBelowMinimumTimesThresholdException exception =
                Assert.ThrowsException<ValueBelowMinimumTimesThresholdException>(
                    () =>
                    validator.Validate(Enumerable.Range(0, expectedTimesSeen).Select(x => value)));

            Assert.AreEqual(value, exception.Value);
            Assert.AreEqual(expectedTimesSeen, exception.TimesSeen);
            Assert.AreEqual(minimumCount, exception.MinimumTimes);
        }

        [TestMethod]
        public void SetValidator_Validate_ValueNotSuppliedButExistsInRestrictionSet_ThrowsValueBelowMinimumTimesThreshold()
        {
            const int expectedExceptionValue = 5;
            const int otherLegalValue = 3;

            SetValidator<int> validator = new SetValidator<int>(
                new SetRestrictionInfo<int>[]
                {
                    new SetRestrictionInfo<int>(otherLegalValue),
                    new SetRestrictionInfo<int>(expectedExceptionValue, minimumTimesAllowed: 1)
                });

            ValueBelowMinimumTimesThresholdException exception =
                Assert.ThrowsException<ValueBelowMinimumTimesThresholdException>(
                    () =>
                    validator.Validate(
                        new int[]
                        {
                            otherLegalValue
                        }));

            Assert.AreEqual(expectedExceptionValue, exception.Value);
            Assert.AreEqual(0, exception.TimesSeen);
            Assert.AreEqual(1, exception.MinimumTimes);
        }

        [TestMethod]
        public void SetValidator_Validate_ValueAboveMaximumTimesThreshold_ThrowsValueAboveMaximumTimesThreshold()
        {
            const int value = 5;
            const int maximumCount = 3;
            const int expectedTimesSeen = maximumCount + 1;

            SetValidator<int> validator = new SetValidator<int>(
                new SetRestrictionInfo<int>[]
                {
                    new SetRestrictionInfo<int>(value, maximumTimesAllowed: maximumCount)
                });

            ValueAboveMaximumTimesThresholdException exception =
                Assert.ThrowsException<ValueAboveMaximumTimesThresholdException>(
                    () =>
                    validator.Validate(Enumerable.Range(0, expectedTimesSeen).Select(x => value)));

            Assert.AreEqual(value, exception.Value);
            Assert.AreEqual(expectedTimesSeen, exception.TimesSeen);
            Assert.AreEqual(maximumCount, exception.MaximumTimes);
        }
    }
}
