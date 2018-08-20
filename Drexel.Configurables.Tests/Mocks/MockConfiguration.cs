using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Tests.Mocks
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Test code.")]
    public class MockConfiguration : IConfiguration
    {
        private IReadOnlyDictionary<IConfigurationRequirement, object> backingMappings;

        public MockConfiguration(
            IConfigurator configurator = null,
            IReadOnlyDictionary<IConfigurationRequirement, object> mappings = null)
        {
            this.Configurator = configurator;
            this.backingMappings = mappings;
        }

        public IConfigurator Configurator { get; private set; }

        public IReadOnlyList<IConfigurationRequirement> Keys => this.backingMappings.Keys.ToList();

        public object this[IConfigurationRequirement requirement] => this.backingMappings[requirement];

        public IEnumerator<IMapping<IConfigurationRequirement>> GetEnumerator() =>
            this.backingMappings.Select(x => new Mapping(x.Key, x.Value)).GetEnumerator();

        public object GetOrDefault(IConfigurationRequirement requirement, Func<object> defaultValueFactory)
        {
            throw new NotImplementedException();
        }

        public bool TryGetOrDefault<T>(IConfigurationRequirement requirement, Func<T> defaultValueFactory, out T result)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
