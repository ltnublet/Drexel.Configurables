using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// A simple implementation of <see cref="IBoundConfiguration"/>.
    /// </summary>
    public class BoundConfiguration : IBoundConfiguration
    {
        /// <summary>
        /// Exception message for when a configuration requirements are null.
        /// </summary>
        internal const string ConfigurableRequirementsMustNotBeNull =
            "Configuration requirements must not be null.";

        /// <summary>
        /// Exception message for when a requirement is missing.
        /// </summary>
        internal const string MissingRequirement =
            "Missing required requirement. Name: '{0}'.";

        /// <summary>
        /// Exception message for when a dependency is not satisfied.
        /// </summary>
        internal const string DependenciesNotSatisfied =
            "Requirement '{0}' does not have its dependencies fulfilled.";

        /// <summary>
        /// Exception message for when a conflicting requirement is specified.
        /// </summary>
        internal const string ConflictingRequirementsSpecified =
            "Requirement '{0}' has conflicting requirements specified.";

        /// <summary>
        /// Exception message for when requirements fail validation.
        /// </summary>
        internal const string RequirementsFailedValidation =
            "Supplied requirements failed validation.";

        private Dictionary<IConfigurationRequirement, object> backingDictionary;
        private Lazy<IBinding[]> backingBindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundConfiguration"/> class.
        /// </summary>
        /// <param name="configurable">
        /// The configurable.
        /// </param>
        /// <param name="bindings">
        /// The bindings.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Occurs when the specified <paramref name="bindings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AggregateException">
        /// Occurs when the specified <paramref name="bindings"/> is invalid; the
        /// <see cref="AggregateException.InnerExceptions"/> are the specific reasons the <paramref name="bindings"/>
        /// were invalid.
        /// </exception>
        public BoundConfiguration(
            IConfigurable configurable,
            IReadOnlyDictionary<IConfigurationRequirement, object> bindings)
        {
            if (configurable == null)
            {
                throw new ArgumentNullException(nameof(configurable));
            }

            if (bindings == null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            if (configurable.Requirements == null)
            {
                throw new ArgumentException(
                    BoundConfiguration.ConfigurableRequirementsMustNotBeNull,
                    nameof(configurable));
            }

            List<Exception> failures = new List<Exception>();
            this.backingDictionary = new Dictionary<IConfigurationRequirement, object>();
            foreach (IConfigurationRequirement requirement in configurable.Requirements)
            {
                bool present = bindings.TryGetValue(requirement, out object binding);
                if (!present && !requirement.IsOptional)
                {
                    failures.Add(
                        new ArgumentException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                BoundConfiguration.MissingRequirement,
                                requirement.Name)));
                }
                else if (present)
                {
                    // TODO: Should validation happen after checking there are no DependsOn/ExclusiveWith failures?
                    Exception exception = requirement.Validate(binding);

                    if (exception != null)
                    {
                        failures.Add(exception);
                    }

                    this.backingDictionary.Add(requirement, binding);
                }
            }

            foreach (KeyValuePair<IConfigurationRequirement, object> pair in
                this.backingDictionary
                    .Where(x => !x.Key.DependsOn.All(y => this.backingDictionary.ContainsKey(y)))
                    .ToArray())
            {
                this.backingDictionary.Remove(pair.Key);
                failures.Add(
                    new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            BoundConfiguration.DependenciesNotSatisfied,
                            pair.Key.Name)));
            }

            foreach (KeyValuePair<IConfigurationRequirement, object> pair in
                this.backingDictionary
                    .Where(x => x.Key.ExclusiveWith.Any(y => this.backingDictionary.ContainsKey(y)))
                    .ToArray())
            {
                this.backingDictionary.Remove(pair.Key);
                failures.Add(
                    new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            BoundConfiguration.ConflictingRequirementsSpecified,
                            pair.Key.Name)));
            }

            if (failures.Any())
            {
                throw new AggregateException(
                    BoundConfiguration.RequirementsFailedValidation,
                    failures);
            }

            this.backingBindings =
                new Lazy<IBinding[]>(() => this.backingDictionary.Select(x => new Binding(x.Key, x.Value)).ToArray());
        }

        /// <summary>
        /// A collection of <see cref="IBinding"/>s contained by this <see cref="IBoundConfiguration"/>.
        /// </summary>
        public IEnumerable<IBinding> Bindings => this.backingBindings.Value;

        /// <summary>
        /// Gets the <see cref="object"/> associated with the specified <see cref="IConfigurationRequirement"/>
        /// <paramref name="requirement"/>.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> associated with the specified <see cref="IConfigurationRequirement"/>
        /// <paramref name="requirement"/>.
        /// </returns>
        public object this[IConfigurationRequirement requirement] => this.backingDictionary[requirement];

        /// <summary>
        /// If the specified <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is contained by
        /// this <see cref="IBoundConfiguration"/>, returns the <see cref="object"/> held by the associated
        /// <see cref="IBinding"/> in <see cref="IBoundConfiguration.Bindings"/>; otherwise, returns the result of invoking
        /// the supplied <paramref name="defaultValueFactory"/>.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <param name="defaultValueFactory">
        /// The default value factory.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> associated with the specified <see cref="IConfigurationRequirement"/> if it is
        /// contained by this <see cref="IBoundConfiguration"/>; otherwise, the value returned by
        /// <paramref name="defaultValueFactory"/>.
        /// </returns>
        public object GetOrDefault(IConfigurationRequirement requirement, Func<object> defaultValueFactory)
        {
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
    }
}
