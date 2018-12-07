using System;
using System.Collections.Generic;
using System.Text;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.RequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests.RequirementTypesTests
{
    [TestClass]
    public class V1Tests
    {
        private Version expectedVersion;

        [TestInitialize]
        public void TestInitialize()
        {
            this.expectedVersion = new Version(1, 0, 0, 0);
        }

        [TestMethod]
        public void V1_Bool_Ctor_Succeeds()
        {
            IRequirementType<bool> asGeneric = V1.Bool;
            IRequirementType asNonGeneric = asGeneric;

            Assert.IsTrue(asGeneric.IsPersistable);
            Assert.AreEqual(typeof(bool), asGeneric.Type);
            Assert.AreEqual(this.expectedVersion, asGeneric.Version);

            Assert.IsTrue(asNonGeneric.IsPersistable);
            Assert.AreEqual(typeof(bool), asNonGeneric.Type);
            Assert.AreEqual(this.expectedVersion, asNonGeneric.Version);
        }

        [DataTestMethod]
        [DataRow(true, true)]
        [DataRow(false, false)]
        public void V1_Bool_Cast_Succeeds(bool expected, bool value)
        {
            IRequirementType<bool> asGeneric = V1.Bool;
            Assert.AreEqual(expected, asGeneric.Cast(value));
        }

        [DataTestMethod]
        [DataRow("true")]
        [DataRow(12)]
        [DataRow(1.234D)]
        [DataRow(null)]
        public void V1_Bool_Cast_ThrowsInvalidCast(object value)
        {
            IRequirementType<bool> asGeneric = V1.Bool;
            Assert.ThrowsException<InvalidCastException>(() => asGeneric.Cast(value));
        }

        [DataTestMethod]
        [DataRow("True", true)]
        [DataRow("False", false)]
        public void V1_Bool_Persist_Succeeds(string expected, bool value)
        {
            IRequirementType<bool> asGeneric = V1.Bool;
            IRequirementType asNonGeneric = asGeneric;

            Assert.AreEqual(expected, asGeneric.Persist(value));
            Assert.AreEqual(expected, asNonGeneric.Persist(value));
        }

        [DataTestMethod]
        [DataRow("true")]
        [DataRow(12)]
        [DataRow(1.234D)]
        [DataRow(null)]
        public void V1_Bool_Persist_ThrowsInvalidCast(object value)
        {
            IRequirementType asNonGeneric = V1.Bool;
            Assert.ThrowsException<InvalidCastException>(() => asNonGeneric.Persist(value));
        }

        [DataTestMethod]
        [DataRow(true, "True")]
        [DataRow(true, "true")]
        [DataRow(true, "TRUE")]
        [DataRow(false, "False")]
        [DataRow(false, "false")]
        [DataRow(false, "FALSE")]
        public void V1_Bool_Restore_Succeeds(bool expected, string value)
        {
            IRequirementType<bool> asGeneric = V1.Bool;
            IRequirementType asNonGeneric = asGeneric;

            Assert.AreEqual(expected, asGeneric.Restore(value));
            Assert.AreEqual(expected, asNonGeneric.Restore(value));
        }

        [DataTestMethod]
        [DataRow("aaaa")]
        [DataRow("1")]
        [DataRow("!@#%##&!*)#@)@#*(*!9018342_+-=")]
        [DataRow("{true}")]
        public void V1_Bool_Restore_ThrowsInvalidCast(string value)
        {
            IRequirementType<bool> asGeneric = V1.Bool;
            IRequirementType asNonGeneric = asGeneric;

            Assert.ThrowsException<InvalidCastException>(() => asGeneric.Restore(value));
            Assert.ThrowsException<InvalidCastException>(() => asNonGeneric.Restore(value));
        }
    }
}
