using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// A simple implementation of <see cref="IConfiguration"/>.
    /// </summary>
    public class Configuration : IConfiguration
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

        private readonly IConfiguration backingConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="requirementSource">
        /// The requirement source.
        /// </param>
        /// <param name="mappings">
        /// The mappings.
        /// </param>
        /// <param name="configurator">
        /// The <see cref="IConfigurator"/> which produced this <see cref="Configuration"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Occurs when the specified <paramref name="mappings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidRequirementsException">
        /// Occurs when the specified <paramref name="requirementSource"/> contains invalid requirements.
        /// </exception>
        /// <exception cref="InvalidMappingsException">
        /// Occurs when the supplied <paramref name="mappings"/> are invalid.
        /// </exception>
        public Configuration(
            IRequirementSource requirementSource,
            IReadOnlyDictionary<IConfigurationRequirement, object> mappings,
            IConfigurator configurator = null)
        {
            if (requirementSource == null)
            {
                throw new ArgumentNullException(nameof(requirementSource));
            }

            if (mappings == null)
            {
                throw new ArgumentNullException(nameof(mappings));
            }

            if (requirementSource.Requirements == null)
            {
                throw new InvalidRequirementsException(
                    Configuration.ConfigurableRequirementsMustNotBeNull,
                    nameof(requirementSource));
            }

            Dictionary<IConfigurationRequirement, object> backingDictionary =
                mappings.ToDictionary(x => x.Key, x => x.Value);

            List<Exception> failures = new List<Exception>();

            // Check for missing requirements.
            failures.AddRange(requirementSource
                .Requirements
                .Where(x => !x.IsOptional && !backingDictionary.Keys.Contains(x))
                .Select(x => new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Configuration.MissingRequirement,
                        x.Name))));

            // Check for DependsOn failures.
            IConfigurationRequirement[] dependsOnFailures = backingDictionary
                .Keys
                .Where(x => x.DependsOn.Any(y => !mappings.Keys.Contains(y)))
                .ToArray();
            failures.AddRange(dependsOnFailures
                .Select(x => new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Configuration.DependenciesNotSatisfied,
                        x.Name))));

            // Check for ExclusiveWith failures.
            IConfigurationRequirement[] exclusiveConflicts = backingDictionary
                .Keys
                .Where(x => x.ExclusiveWith.Any(y => mappings.Keys.Contains(y)))
                .ToArray();
            failures.AddRange(exclusiveConflicts
                .Select(x => new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Configuration.ConflictingRequirementsSpecified,
                        x.Name))));

            // Remove ExclusiveWith/DependsOn errors from the set of validations to execute.
            foreach (IConfigurationRequirement cantValidate in exclusiveConflicts.Concat(dependsOnFailures))
            {
                backingDictionary.Remove(cantValidate);
            }

            // Validate the remaining requirements.
            // We can only reach this after removing all requirements that don't have their prerequisites.
            // We need to determine what "level" in the heirarchy each requirement is at.
            // If a requirement has no dependsOns, then it's at level "0" (can be immediately evaluated)
            // For each requirement which has all dependsOns in the set of previously evaluated requirements, add it
            // Repeat until all requirements have been added (this can always be reached, because we removed all
            // requirements which didn't have all their dependsOns satisfied)
            IList<IConfigurationRequirement> toEvaluate = new List<IConfigurationRequirement>();
            void RecursiveAdd(
                ref IList<IConfigurationRequirement> addTo,
                IReadOnlyList<IConfigurationRequirement> remaining)
            {
                if (!remaining.Any())
                {
                    return;
                }

                foreach (IConfigurationRequirement toCheck in remaining)
                {
                    if (!toCheck.DependsOn.Any() || toCheck.DependsOn.All(x => toEvaluate.Contains(x)))
                    {
                        toEvaluate.Add(toCheck);
                    }
                }
            }

            RecursiveAdd(ref toEvaluate, backingDictionary.Keys.ToList());

            // We can now iterate through toEvaluate; at each requirement, we set of previously completed requirements
            // will satisfy all dependsOns.
            Dictionary<IConfigurationRequirement, object> completed =
                new Dictionary<IConfigurationRequirement, object>();
            foreach (IConfigurationRequirement requirement in toEvaluate)
            {
                object value = mappings[requirement];

                Exception failure = requirement.Validate(
                    value,
                    new SimpleConfiguration(completed
                        .Where(x => requirement.DependsOn.Contains(x.Key))
                        .ToDictionary(x => x.Key, x => x.Value)));

                if (failure == null)
                {
                    completed.Add(requirement, new Mapping(requirement, value));
                }
                else
                {
                    failures.Add(failure);

                    // This isn't really required, but just for future-proofing...
                    backingDictionary.Remove(requirement);
                }
            }

            if (failures.Any())
            {
                throw new AggregateException(
                    Configuration.RequirementsFailedValidation,
                    failures);
            }

            this.backingConfiguration = new SimpleConfiguration(completed, configurator);
        }

        public IConfigurator Configurator => this.backingConfiguration.Configurator;

        public object this[IConfigurationRequirement requirement] => this.backingConfiguration[requirement];

        public object GetOrDefault(IConfigurationRequirement requirement, Func<object> defaultValueFactory) =>
            this.backingConfiguration.GetOrDefault(requirement, defaultValueFactory);

        public bool TryGetOrDefault<T>(
            IConfigurationRequirement requirement,
            Func<T> defaultValueFactory,
            out T result)
        {
            return this.backingConfiguration.TryGetOrDefault<T>(requirement, defaultValueFactory, out result);
        }

        public IEnumerator<IMapping> GetEnumerator() => this.backingConfiguration.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
