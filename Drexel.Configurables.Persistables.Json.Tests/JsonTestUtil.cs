using System;
using System.Linq;
using Drexel.Configurables.Persistables.Contracts;
using Drexel.Configurables.Tests.Common;

namespace Drexel.Configurables.Persistables.Json.Tests
{
    public static class JsonTestUtil
    {
        public static IPersistableConfiguration CreateSampleConfiguration()
        {
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

            return configuration;
        }
    }
}
