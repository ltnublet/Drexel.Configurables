using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Tests.Mocks
{
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

        public object this[IConfigurationRequirement requirement] => this.backingMappings[requirement];

        public IConfigurator Configurator { get; private set; }

        public IEnumerator<IMapping> GetEnumerator() =>
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
