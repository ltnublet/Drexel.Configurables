using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Persistables.Contracts;
using Drexel.Configurables.Tests.Common;

namespace Drexel.Configurables.Persistables.Json.Tests
{
    internal class TestRequirementSource : IPersistableConfigurationRequirementSource
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
