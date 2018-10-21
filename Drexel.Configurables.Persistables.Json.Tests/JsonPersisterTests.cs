using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Persistables;
using Drexel.Configurables.Persistables.Contracts;
using Drexel.Configurables.Persistables.Json;
using Drexel.Configurables.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexexl.Configurables.Persistables.Json.Tests.NetCore
{
    [TestClass]
    public class JsonPersisterTests
    {
        [TestMethod]
        public void JsonPersistor_Ctor_Succeeds()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                JsonPersister persister = new JsonPersister(stream);
                Assert.IsNotNull(persister);
            }
        }

        [TestMethod]
        public void JsonPersister_Ctor_ThrowsArgumentNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new JsonPersister(null));
        }

        [TestMethod]
        public async Task JsonPersister_Persist_Succeeds()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                JsonPersister persister = new JsonPersister(stream);

                TestRequirementSource source = new TestRequirementSource(
                    Enumerable.Range(0, 10)
                        .Select(x => TestUtil.CreateConfigurationRequirement())
                        .Select(x => new PersistableConfigurationRequirement(
                            Guid.NewGuid(),
                            new Version(1, 0),
                            x,
                            new PersistableConfigurationRequirementType(
                                x.OfType,
                                new Version(1, 0),
                                y => y.ToString(),
                                y => y)))
                        .ToArray());

                PersistableConfiguration configuration = new PersistableConfiguration(
                    source,
                    source.GenerateValidMappings());

                await persister.PersistAsync(configuration, CancellationToken.None);

                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    Assert.IsFalse(string.IsNullOrWhiteSpace(content));
                }
            }
        }

        private class TestRequirementSource : IPersistableConfigurationRequirementSource
        {
            public TestRequirementSource(params IPersistableConfigurationRequirement[] requirements)
            {
                this.Requirements = requirements?.ToList();
            }

            public IReadOnlyList<IPersistableConfigurationRequirement> Requirements { get; }

            IReadOnlyList<IConfigurationRequirement> IRequirementSource.Requirements => this.Requirements;

            public IReadOnlyDictionary<IPersistableConfigurationRequirement, object> GenerateValidMappings()
            {
                return (IReadOnlyDictionary<IPersistableConfigurationRequirement, object>)this
                    .Requirements
                    .ToDictionary(
                        x => x,
                        x => TestUtil.GetDefaultValidObjectForRequirement(x));
            }
        }
    }
}
