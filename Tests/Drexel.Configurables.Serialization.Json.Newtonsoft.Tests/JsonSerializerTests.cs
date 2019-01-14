using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.Tests
{
    [TestClass]
    public class JsonSerializerTests
    {
        [TestMethod]
        public void JsonSerializer_Ctor_Succeeds()
        {
            JsonSerializer serializer = new JsonSerializer(
                StandardTypeSerializers.StandardSerializers,
                new MemoryStream());

            serializer.Dispose();
        }

        [TestMethod]
        public async Task JsonSerializer_SerializeAsync_Succeeds()
        {
            List<KeyValuePair<Requirement, object?>> values =
                new List<KeyValuePair<Requirement, object?>>();
            List<KeyValuePair<Requirement, IEnumerable?>> collections =
                new List<KeyValuePair<Requirement, IEnumerable?>>();
            foreach (RequirementType type in StandardRequirementTypes.StandardTypes)
            {
                values.Add(
                    new KeyValuePair<Requirement, object?>(
                        type.GetValidRequirement(false), 
                        type.GetValidValue()));
                collections.Add(
                    new KeyValuePair<Requirement, IEnumerable?>(
                        type.GetValidRequirement(true),
                        type.GetValidCollection()));
            }

            RequirementSet requirements = new RequirementSet(
                values.Select(x => x.Key).Concat(collections.Select(x => x.Key)).ToArray());

            ConfigurationBuilder builder = new ConfigurationBuilder();
            foreach (KeyValuePair<Requirement, object?> value in values)
            {
                builder.Add(value.Key, value.Value);
            }

            foreach (KeyValuePair<Requirement, IEnumerable?> collection in collections)
            {
                builder.Add(collection.Key, collection.Value);
            }

            Configuration configuration = await builder.Build(requirements);

            MemoryStream memoryStream = new MemoryStream();
            using (JsonSerializer serializer = new JsonSerializer(
                StandardTypeSerializers.StandardSerializers,
                memoryStream,
                leaveOpen: true))
            {
                await serializer.SerializeAsync(configuration);
            }

            string? content = null;
            memoryStream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                content = reader.ReadToEnd();
            }
        }
    }
}
