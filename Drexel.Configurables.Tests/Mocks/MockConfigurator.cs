using System.Collections.Generic;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Tests.Mocks
{
    public class MockConfigurator : IConfigurator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Test code.")]
        public delegate IConfiguration ConfigureFunc(
            IRequirementSource source,
            IReadOnlyDictionary<IConfigurationRequirement, object> bindings);

        private IRequirementSource source;
        private ConfigureFunc configureFunc;

        public MockConfigurator(IRequirementSource source, ConfigureFunc configureFunc)
        {
            this.source = source;
            this.configureFunc = configureFunc;
        }

        public IConfiguration Configure(IReadOnlyDictionary<IConfigurationRequirement, object> mappings) =>
            this.configureFunc?.Invoke(this.source, mappings);
    }
}
