using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Tests.Common.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.External.Tests
{
    [TestClass]
    public class FilePathTests
    {
        private static MockPathInteractor interactor;

        public static IEnumerable<object[]> FileNameTestCases { get; } =
            new object[][]
            {
                new object[]
                {
                    new string[]
                    {
                        "PaTh.TxT",
                        "paTH.TXt",
                        "path.txt",
                        "PATH.TXT",
                        "PAth.txT",
                        "pAtH.tXt"
                    }
                }
            };

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            FilePathTests.interactor = new MockPathInteractor(x => x, x => true);
        }

        [TestMethod]
        public void FilePath_Ctor_PathNotAbsolute()
        {
            const string noRoot = "example.txt";
            const string root = @"C:\example.txt";

            MockPathInteractor interactor = new MockPathInteractor(
                x => root,
                x => true);

            ArgumentException exception =
                Assert.ThrowsException<ArgumentException>(() => new FilePath(noRoot, interactor));
            StringAssert.Contains(exception.Message, FilePath.InvalidPath);
        }

        [TestMethod]
        public void FilePath_Ctor_AbsoluteButNotRooted()
        {
            const string path = "path";

            MockPathInteractor interactor = new MockPathInteractor(
                x => x,
                x => false);

            ArgumentException exception =
                Assert.ThrowsException<ArgumentException>(() => new FilePath(path, interactor));
            StringAssert.Contains(exception.Message, FilePath.InvalidPath);
        }

        [TestMethod]
        public void FilePath_Ctor_NullPath()
        {
            MockPathInteractor interactor = new MockPathInteractor(null, null);

            Assert.ThrowsException<ArgumentNullException>(() => new FilePath(null, interactor));
        }

        [TestMethod]
        public void FilePath_Ctor_NullInteractor()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FilePath("not null", null));
        }

        [TestMethod]
        public void FilePath_Ctor_Valid()
        {
            const string path = "path";

            FilePath filePath = new FilePath(path, FilePathTests.interactor);
            Assert.IsNotNull(filePath);
        }

        [TestMethod]
        public void FilePath_Ctor_InteractorThrowsException()
        {
            const string path = "path";

            MockPathInteractor interactor = new MockPathInteractor(
                x => throw new NotImplementedException(),
                x => true);

            ArgumentException exception =
                Assert.ThrowsException<ArgumentException>(() => new FilePath(path, interactor));
            StringAssert.Contains(exception.Message, FilePath.InvalidPath);
        }

        [TestMethod]
        public void FilePath_ToString_Succeeds()
        {
            const string path = "Path123.txt";

            FilePath filePath = new FilePath(path, FilePathTests.interactor);

            Assert.AreEqual(path, filePath.ToString());
        }

        [TestMethod]
        public void FilePath_GetHashCode_Succeeds()
        {
            const string path1 = "Path1.txt";
            const string path2 = "Path2.txt";

            FilePath filePath1 = new FilePath(path1, FilePathTests.interactor);
            FilePath filePath2 = new FilePath(path2, FilePathTests.interactor);

            int hashCode1 = filePath1.GetHashCode();
            int hashCode2 = filePath2.GetHashCode();

            Assert.AreNotEqual(hashCode1, hashCode2);
        }

        [DataTestMethod]
        [DynamicData(nameof(FilePathTests.FileNameTestCases))]
        public void FilePath_GetHashCode_CaseInsensitive(string[] fileNames)
        {
            fileNames
                .Select(x => new FilePath(x, FilePathTests.interactor).GetHashCode())
                .Distinct()
                .Single();
        }

        [DataTestMethod]
        [DynamicData(nameof(FilePathTests.FileNameTestCases))]
        public void FilePath_GetHashCode_CaseSensitive(string[] fileNames)
        {
            Assert.AreEqual(
                fileNames.Length,
                fileNames
                    .Select(x => new FilePath(x, FilePathTests.interactor, true).GetHashCode())
                    .Distinct()
                    .Count());
        }

        [TestMethod]
        public void FilePath_Equals_Succeeds()
        {
            const string path = "path.txt";

            FilePath filePath1 = new FilePath(path, FilePathTests.interactor);
            FilePath filePath2 = new FilePath(path, FilePathTests.interactor);

            Assert.IsTrue(filePath1.Equals(filePath2));
            Assert.IsTrue(filePath2.Equals(filePath1));
        }

        [TestMethod]
        public void FilePath_Equals_Null()
        {
            const string path = "path.txt";

            FilePath filePath1 = new FilePath(path, FilePathTests.interactor);

            Assert.IsFalse(filePath1.Equals(null));
        }

        [TestMethod]
        public void FilePath_Equals_CaseSensitivityConflictResolved()
        {
            const string path1 = "path.txt";
            const string path2 = "PATH.TXT";

            FilePath caseInsensitive = new FilePath(path1, FilePathTests.interactor);
            FilePath caseSensitive = new FilePath(path2, FilePathTests.interactor, true);

            Assert.IsFalse(caseInsensitive.Equals(caseSensitive));
            Assert.IsFalse(caseSensitive.Equals(caseInsensitive));
        }

        [TestMethod]
        public void FilePath_Equals_CaseSensitivityHonored()
        {
            const string path1 = "path.txt";
            const string path2 = "PATH.TXT";

            FilePath lowerCase = new FilePath(path1, FilePathTests.interactor);
            FilePath upperCase = new FilePath(path2, FilePathTests.interactor);

            Assert.IsTrue(lowerCase.Equals(upperCase));
            Assert.IsTrue(upperCase.Equals(lowerCase));
        }
    }
}