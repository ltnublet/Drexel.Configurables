using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;

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

        private readonly Dictionary<IConfigurationRequirement, object> backingDictionary;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Maintainability",
            "CA1506:AvoidExcessiveClassCoupling",
            Justification = "Intentional.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Maintainability",
            "CA1502:AvoidExcessiveComplexity",
            Justification = "Constructor performs complex logic to validate state.")]
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

            this.backingDictionary = bindings.ToDictionary(x => x.Key, x => x.Value);

            List<Exception> failures = new List<Exception>();

            // Check for missing requirements.
            failures.AddRange(configurable
                .Requirements
                .Where(x => !x.IsOptional && !this.backingDictionary.Keys.Contains(x))
                .Select(x => new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        BoundConfiguration.MissingRequirement,
                        x.Name))));

            // Check for DependsOn failures.
            IConfigurationRequirement[] dependsOnFailures = this.backingDictionary
                .Keys
                .Where(x => x.DependsOn.Any(y => !bindings.Keys.Contains(y)))
                .ToArray();
            failures.AddRange(dependsOnFailures
                .Select(x => new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        BoundConfiguration.DependenciesNotSatisfied,
                        x.Name))));

            // Check for ExclusiveWith failures.
            IConfigurationRequirement[] exclusiveConflicts = this.backingDictionary
                .Keys
                .Where(x => x.ExclusiveWith.Any(y => bindings.Keys.Contains(y)))
                .ToArray();
            failures.AddRange(exclusiveConflicts
                .Select(x => new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        BoundConfiguration.ConflictingRequirementsSpecified,
                        x.Name))));

            // Remove ExclusiveWith/DependsOn errors from the set of validations to execute.
            foreach (IConfigurationRequirement cantValidate in exclusiveConflicts.Concat(dependsOnFailures))
            {
                this.backingDictionary.Remove(cantValidate);
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

            RecursiveAdd(ref toEvaluate, this.backingDictionary.Keys.ToList());

            // We can now iterate through toEvaluate; at each requirement, we set of previously completed requirements
            // will satisfy all dependsOns.
            Dictionary<IConfigurationRequirement, IBinding> completed =
                new Dictionary<IConfigurationRequirement, IBinding>();
            foreach (IConfigurationRequirement requirement in toEvaluate)
            {
                object value = bindings[requirement];

                Exception failure = requirement.Validate(
                    value,
                    completed.Where(
                        x =>
                        requirement.DependsOn.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value));

                if (failure == null)
                {
                    completed.Add(requirement, new Binding(requirement, value));
                }
                else
                {
                    failures.Add(failure);

                    // This isn't really required, but just for future-proofing...
                    this.backingDictionary.Remove(requirement);
                }
            }

            if (failures.Any())
            {
                throw new AggregateException(
                    BoundConfiguration.RequirementsFailedValidation,
                    failures);
            }

            this.Bindings = completed.Values;
        }

        /// <summary>
        /// A collection of <see cref="IBinding"/>s contained by this <see cref="IBoundConfiguration"/>.
        /// </summary>
        public IEnumerable<IBinding> Bindings { get; private set; }

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
        /// <see cref="IBinding"/> in <see cref="IBoundConfiguration.Bindings"/>; otherwise, returns the result of
        /// invoking the supplied <paramref name="defaultValueFactory"/>.
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
        /// If the specified <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is contained by
        /// this <see cref="IBoundConfiguration"/>, <paramref name="result"/> is set to the <typeparamref name="T"/>
        /// held by the associated <see cref="IBinding"/> in <see cref="IBoundConfiguration.Bindings"/>, and
        /// <see langword="true"/> is returned; otherwise, <paramref name="result"/> is set to the result of
        /// invoking the supplied <paramref name="defaultValueFactory"/>, and <see langword="false"/> is returned.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <param name="defaultValueFactory">
        /// The default value factory.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <typeparam name="T">
        /// The expected <see cref="Type"/> of the <see cref="object"/> to return. If the specified
        /// <see cref="IConfigurationRequirement"/> is contained by this <see cref="IBoundConfiguration"/>, but the
        /// <paramref name="result"/> cannot be set to the expected <see cref="Type"/> <typeparamref name="T"/>, then
        /// the <paramref name="defaultValueFactory"/> will be invoked.
        /// </typeparam>
        /// <returns>
        /// <see langword="true"/> if the <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is
        /// contained by this <see cref="IBoundConfiguration"/>, and <paramref name="result"/> is able to be
        /// set to the expected <see cref="Type"/> <typeparamref name="T"/>; otherwise, return <see langword="false"/>.
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
    }
}
