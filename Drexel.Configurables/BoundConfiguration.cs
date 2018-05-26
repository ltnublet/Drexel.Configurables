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

        private readonly Lazy<IBinding[]> backingBindings;
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

            // Build dependsOn chains.
            Dictionary<IConfigurationRequirement, List<IConfigurationRequirement>> chains =
                new Dictionary<IConfigurationRequirement, List<IConfigurationRequirement>>();
            Dictionary<IConfigurationRequirement, TreeNode<IConfigurationRequirement>> nodes =
                new Dictionary<IConfigurationRequirement, TreeNode<IConfigurationRequirement>>();
            foreach (IConfigurationRequirement requirement in this.backingDictionary.Keys)
            {
                chains.Add(
                    requirement,
                    this.backingDictionary.Keys.Where(x => x.DependsOn.Contains(requirement)).ToList());
                nodes.Add(
                    requirement,
                    new TreeNode<IConfigurationRequirement>(requirement));
            }

            foreach (KeyValuePair<IConfigurationRequirement, TreeNode<IConfigurationRequirement>> requirement in nodes)
            {
                requirement.Value.AddRange(chains[requirement.Key].Select(child => nodes[child]));
            }

            // At this point, we now have a tree for each requirement, where performing a BFS and providing all
            // previously computed nodes at that depth will satisfy the requirements at each level.
            IConfigurationRequirement[] roots = this.backingDictionary.Keys.Where(x => !x.DependsOn.Any()).ToArray();
            Dictionary<IConfigurationRequirement, IBinding> completed =
                new Dictionary<IConfigurationRequirement, IBinding>();
            void PerformValidation(IEnumerable<TreeNode<IConfigurationRequirement>> currentLayer)
            {
                List<TreeNode<IConfigurationRequirement>> asList = currentLayer.ToList();
                if (!asList.Any())
                {
                    // Base case - current depth contains no nodes.
                    return;
                }

                foreach (IConfigurationRequirement requirement in asList.Select(x => x.Value))
                {
                    Exception exception = BoundConfiguration.Validate(bindings, requirement, out object binding);
                    if (exception == null)
                    {
                        completed.Add(requirement, new Binding(requirement, binding));
                    }
                    else
                    {
                        failures.Add(exception);
                    }
                }

                PerformValidation(asList.SelectMany(x => x.Children).ToList());
            }

            PerformValidation(nodes.Where(x => roots.Contains(x.Key)).Select(x => x.Value));

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

        private static Exception Validate(
            IReadOnlyDictionary<IConfigurationRequirement, object> bindings,
            IConfigurationRequirement requirement,
            out object binding)
        {
            bool present = bindings.TryGetValue(requirement, out binding);
            if (!present && !requirement.IsOptional)
            {
                return new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        BoundConfiguration.MissingRequirement,
                        requirement.Name));
            }
            else if (present)
            {
                Exception exception = requirement.Validate(binding);

                return exception;
            }

            return null;
        }

        private class TreeNode<T>
        {
            private readonly List<TreeNode<T>> innerChildren;

            public TreeNode(T value)
            {
                this.Value = value;
                this.innerChildren = new List<TreeNode<T>>();
            }

            public T Value { get; private set; }

            public IReadOnlyList<TreeNode<T>> Children => this.innerChildren;

            public void AddRange(IEnumerable<TreeNode<T>> children)
            {
                this.innerChildren.AddRange(children);
            }
        }
    }
}
