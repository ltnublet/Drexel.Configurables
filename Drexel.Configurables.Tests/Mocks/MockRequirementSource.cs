using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Tests.Mocks
{
    public class MockRequirementSource : IRequirementSource
    {
        private readonly Lazy<IReadOnlyList<IConfigurationRequirement>> requirements;

        public MockRequirementSource(IEnumerable<IConfigurationRequirement> requirements)
        {
            this.requirements = new Lazy<IReadOnlyList<IConfigurationRequirement>>(() => requirements?.ToList());
        }

        public IReadOnlyList<IConfigurationRequirement> Requirements => this.requirements.Value;
    }
}
