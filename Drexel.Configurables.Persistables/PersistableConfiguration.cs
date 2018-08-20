using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Persistables.Contracts;

namespace Drexel.Configurables.Persistables
{
    public sealed class PersistableConfiguration : IPersistableConfiguration
    {
        private readonly Configuration configuration;

        public PersistableConfiguration(
            IPersistableConfigurationRequirementSource requirementSource,
            IReadOnlyDictionary<IPersistableConfigurationRequirement, object> mappings,
            IConfigurator configurator = null)
        {
            Dictionary<IPersistableConfigurationRequirement, IConfigurationRequirement> upcast =
                mappings.Keys.ToDictionary(x => x, x => (IConfigurationRequirement)x);
            Dictionary<IConfigurationRequirement, IPersistableConfigurationRequirement> downcast =
                upcast.ToDictionary(x => x.Value, x => x.Key);

            this.configuration = new Configuration(
                requirementSource,
                mappings?.ToDictionary(x => upcast[x.Key], x => x.Value),
                configurator);

            this.Keys = this.configuration.Keys.Select(x => downcast[x]).ToList();
        }

        public object this[IConfigurationRequirement requirement] => this.configuration[requirement];

        public IReadOnlyList<IPersistableConfigurationRequirement> Keys { get; }

        public IConfigurator Configurator => this.configuration.Configurator;

        IReadOnlyList<IConfigurationRequirement> IConfiguration.Keys => this.Keys;

        public IEnumerator<IMapping<IPersistableConfigurationRequirement>> GetEnumerator() => this.GetEnumerator();

        public object GetOrDefault(IConfigurationRequirement requirement, Func<object> defaultValueFactory) =>
            this.configuration.GetOrDefault(requirement, defaultValueFactory);

        public bool TryGetOrDefault<T>(
            IConfigurationRequirement requirement,
            Func<T> defaultValueFactory,
            out T result) => this.configuration.TryGetOrDefault(
                requirement,
                defaultValueFactory,
                out result);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        IEnumerator<IMapping<IConfigurationRequirement>>IEnumerable<IMapping<IConfigurationRequirement>>.GetEnumerator() =>
            this.GetEnumerator();
    }
}
