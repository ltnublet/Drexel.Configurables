using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Tests.Mocks
{
    public class MockConfigurable : IConfigurable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "By design.")]
        public delegate IBoundConfiguration ConfigureFunc(
            IConfigurable self,
            IReadOnlyDictionary<IConfigurationRequirement, object> collection);

        private Lazy<IReadOnlyList<IConfigurationRequirement>> requirements;
        private ConfigureFunc configureFunc;

        public MockConfigurable(
            IEnumerable<IConfigurationRequirement> requirements,
            ConfigureFunc configureFunc = null)
        {
            this.requirements = new Lazy<IReadOnlyList<IConfigurationRequirement>>(() => requirements?.ToList());
            this.configureFunc = configureFunc;
        }

        public IReadOnlyList<IConfigurationRequirement> Requirements => this.requirements.Value;

        public IBoundConfiguration Configure(IReadOnlyDictionary<IConfigurationRequirement, object> bindings) =>
            this.configureFunc?.Invoke(this, bindings);
    }
}
