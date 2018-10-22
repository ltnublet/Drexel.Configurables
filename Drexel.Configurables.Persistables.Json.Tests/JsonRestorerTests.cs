using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Persistables.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Persistables.Json.Tests
{
    [TestClass]
    public class JsonRestorerTests
    {
        [TestMethod]
        public async Task JsonRestorer_RestoreAsync_Succeeds()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IPersistableConfiguration expected = JsonTestUtil.CreateSampleConfiguration();

                // TODO: find a better way to do this? Although I guess it's OK because this test is a "round trip".
                using (JsonPersister persister = new JsonPersister(stream))
                {
                    await persister.PersistAsync(expected, CancellationToken.None);
                }

                stream.Seek(0, SeekOrigin.Begin);

                JsonRestorer restorer = new JsonRestorer(stream);
                IPersistableConfiguration actual = await restorer.RestoreAsync(CancellationToken.None);

                JsonRestorerTests.AssertAreEqual(expected, actual);
            }
        }

        private static void AssertAreEqual(IPersistableConfiguration expected, IPersistableConfiguration actual)
        {
            throw new NotImplementedException();
        }
    }
}
