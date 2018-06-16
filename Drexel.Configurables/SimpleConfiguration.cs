using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// Internal.
    /// </summary>
    internal sealed class SimpleConfiguration : IConfiguration
    {
        private readonly IReadOnlyDictionary<IConfigurationRequirement, object> backingDictionary;
        private readonly IEnumerable<IMapping> backingMappings;

        /// <summary>
        /// Internal.
        /// </summary>
        /// <param name="mappings">
        /// Internal.
        /// </param>
        /// <param name="configurator">
        /// Internal.
        /// </param>
        public SimpleConfiguration(
            IReadOnlyDictionary<IConfigurationRequirement, object> mappings,
            IConfigurator configurator = null)
        {
            this.backingDictionary = mappings.ToDictionary(x => x.Key, x => x.Value);
            this.backingMappings = mappings.Select(x => new Mapping(x.Key, x.Value)).ToList();
            this.Configurator = configurator;
        }

        /// <summary>
        /// Internal.
        /// </summary>
        public IConfigurator Configurator { get; private set; }

        /// <summary>
        /// Internal.
        /// </summary>
        /// <param name="requirement">
        /// Internal.
        /// </param>
        /// <returns>
        /// Internal.
        /// </returns>
        public object this[IConfigurationRequirement requirement] => this.backingDictionary[requirement];

        /// <summary>
        /// Internal.
        /// </summary>
        /// <returns>
        /// Internal.
        /// </returns>
        public IEnumerator<IMapping> GetEnumerator() => this.backingMappings.GetEnumerator();

        /// <summary>
        /// Internal.
        /// </summary>
        /// <param name="requirement">
        /// Internal.
        /// </param>
        /// <param name="defaultValueFactory">
        /// Internal.
        /// </param>
        /// <returns>
        /// Internal.
        /// </returns>
        public object GetOrDefault(IConfigurationRequirement requirement, Func<object> defaultValueFactory)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (defaultValueFactory == null)
            {
                throw new ArgumentNullException(nameof(defaultValueFactory));
            }

            if (!this.backingDictionary.TryGetValue(requirement, out object result))
            {
                result = defaultValueFactory.Invoke();
            }

            return result;
        }

        /// <summary>
        /// Internal.
        /// </summary>
        /// <typeparam name="T">
        /// Internal.
        /// </typeparam>
        /// <param name="requirement">
        /// Internal.
        /// </param>
        /// <param name="defaultValueFactory">
        /// Internal.
        /// </param>
        /// <param name="result">
        /// Internal.
        /// </param>
        /// <returns>
        /// Internal.
        /// </returns>
        public bool TryGetOrDefault<T>(
            IConfigurationRequirement requirement,
            Func<T> defaultValueFactory,
            out T result)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (defaultValueFactory == null)
            {
                throw new ArgumentNullException(nameof(defaultValueFactory));
            }

            if (this.backingDictionary.TryGetValue(requirement, out object actual))
            {
                if (actual is T buffer)
                {
                    result = buffer;
                    return true;
                }
            }

            result = defaultValueFactory.Invoke();
            return false;
        }

        /// <summary>
        /// Internal.
        /// </summary>
        /// <returns>
        /// Internal.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
