using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Persistables.Contracts;
using Drexel.Configurables.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Persistables.Json.Tests.NetCore
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
        public async Task JsonPersister_PersistAsync_Succeeds()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                JsonPersister persister = new JsonPersister(stream);

                IPersistableConfiguration configuration = JsonTestUtil.CreateSampleConfiguration();

                await persister.PersistAsync(configuration, CancellationToken.None);

                JsonPersisterTests.AssertAreEqual(configuration, stream);
            }
        }

        private static void AssertAreEqual(IPersistableConfiguration configuration, Stream stream)
        {
            // TODO: do some real comparison here
            stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(stream))
            {
                string content = reader.ReadToEnd();
                Assert.IsFalse(string.IsNullOrWhiteSpace(content));
            }
        }
    }
}
