using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Drexel.Configurables.Tests
{
    internal static class JsonCompare
    {
        public static void Compare(
            ConfigurationRequirement expected,
            string actual)
        {
            JObject jObj = JObject.Parse(actual);

            Assert.AreEqual(7, jObj.Count);
            Assert.AreEqual(expected.Name, jObj["Name"]);
            Assert.AreEqual(expected.Description, jObj["Description"]);
            Assert.AreEqual(expected.IsOptional, jObj["IsOptional"]);
            Assert.AreEqual(expected.OfType.Type.FullName, jObj["OfType"]);
            JsonCompare.Compare(expected.CollectionInfo, jObj["CollectionInfo"]);
            JsonCompare.Compare(expected.DependsOn, jObj.Value<JArray>("DependsOn"));
            JsonCompare.Compare(expected.ExclusiveWith, jObj.Value<JArray>("ExclusiveWith"));
        }

        private static void Compare(CollectionInfo expected, object actual)
        {
            if (actual is JValue isNull)
            {
                Assert.AreEqual(null, isNull.Value);
            }
            else if (actual is JObject notNull)
            {
                Assert.AreEqual(
                    expected.MinimumCount.ToString(),
                    ((JValue)(notNull["MinimumCount"])).Value);
                Assert.AreEqual(
                    expected.MaximumCount.HasValue
                        ? expected.MaximumCount.Value.ToString()
                        : null,
                    ((JValue)(notNull["MaximumCount"])).Value);
            }
            else
            {
                throw new InternalTestFailureException("Unexpected type.");
            }
        }

        private static void Compare(IEnumerable<IConfigurationRequirement> expected, JArray actual)
        {
            CollectionAssert.AreEqual(expected.ToArray(), actual.Select(x => x.Value<object>()).ToArray());
        }
    }
}
