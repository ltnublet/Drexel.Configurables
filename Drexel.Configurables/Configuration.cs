using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// Represents a validated set of <see cref="IRequirement"/>s, and their associated mapped values.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "CA1710:Identifiers should have correct suffix",
        Justification = "It is not a collection of configurations, and 'RequirementCollection' does not convey that validation has occurred.")]
    public sealed class Configuration : IConfiguration
    {
        /// <summary>
        /// Exception message for when a requirement is missing.
        /// </summary>
        internal const string MissingRequirement =
            "Missing required requirement. Name: '{0}', ID: '{1}'.";

        /// <summary>
        /// Exception message for when a dependency is not satisfied.
        /// </summary>
        internal const string DependenciesNotSatisfied =
            "Requirement does not have its dependencies satisfied. Name: '{0}', ID: '{1}'.";

        /// <summary>
        /// Exception message for when a conflicting requirement is specified.
        /// </summary>
        internal const string ConflictingRequirementsSpecified =
            "Requirement is in conflict with another requirement. Name: '{0}', ID: '{1}'.";

        /// <summary>
        /// Exception message for when requirements fail validation.
        /// </summary>
        internal const string RequirementsFailedValidation =
            "Supplied requirements failed validation.";

        /// <summary>
        /// Exception message for when a mapped value is illegally null.
        /// </summary>
        internal const string MappedValueNullButRequirementNotNullable =
            "Mapped value is null, but the associated requirement type is not nullable.";

        /// <summary>
        /// Exception message for when a mapped value is not enumerable, but the associated requirement has an
        /// enumerable info specified.
        /// </summary>
        internal const string MappedValueIsNotEnumerableButRequirementHasEnumerableInfo =
            "Mapped value is not enumerable, but the associated requirement has enumerable info.";

        /// <summary>
        /// Exception message for when a mapped value exceeds the associated requirement's enumerable info's maximum
        /// count.
        /// </summary>
        internal const string MappedValueExceedsRequirementMaximumCount =
            "Mapped value exceeds associated requirement's enumerable info's maximum count.";

        /// <summary>
        /// Exception message for when a mapped value does not meet the associated requirement's enumerable info's
        /// minimum count.
        /// </summary>
        internal const string MappedValueDoesNotMeetRequirementMinimumCount =
            "Mapped value does not meet associated requirement's enumerable info's minimum count.";

        /// <summary>
        /// Exception message for when a mapped value is a collection which contains a value of an illegal type.
        /// </summary>
        internal const string CollectionContainsValueOfIllegalType =
            "Mapped value contains value of illegal type.";

        private readonly IReadOnlyDictionary<IRequirement, object> mappingDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="requirements">
        /// The set of requirements that the <paramref name="mappings"/> must satisfy.
        /// </param>
        /// <param name="mappings">
        /// The set of mappings between requirements and values.
        /// </param>
        public Configuration(
            IReadOnlyCollection<IRequirement> requirements,
            IReadOnlyDictionary<IRequirement, object> mappings)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            if (mappings == null)
            {
                throw new ArgumentNullException(nameof(mappings));
            }

            Dictionary<IRequirement, object> backingDictionary =
                mappings.ToDictionary(x => x.Key, x => x.Value);

            List<Exception> failures = new List<Exception>();

            // Check for missing requirements.
            failures.AddRange(requirements
                .Where(x => !x.IsOptional && !backingDictionary.Keys.Contains(x))
                .Select(x => new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Configuration.MissingRequirement,
                        x.Name,
                        x.Id))));

            // Check for DependsOn failures.
            IRequirement[] dependsOnFailures = backingDictionary
                .Keys
                .Where(x => x.DependsOn.Any(y => !mappings.Keys.Contains(y)))
                .ToArray();
            failures.AddRange(dependsOnFailures
                .Select(x => new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Configuration.DependenciesNotSatisfied,
                        x.Name,
                        x.Id))));

            // Check for ExclusiveWith failures.
            IRequirement[] exclusiveConflicts = backingDictionary
                .Keys
                .Where(x => x.ExclusiveWith.Any(y => mappings.Keys.Contains(y)))
                .ToArray();
            failures.AddRange(exclusiveConflicts
                .Select(x => new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Configuration.ConflictingRequirementsSpecified,
                        x.Name,
                        x.Id))));

            // Remove ExclusiveWith/DependsOn errors from the set of validations to execute.
            foreach (IRequirement cantValidate in exclusiveConflicts.Concat(dependsOnFailures))
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
            IList<IRequirement> toEvaluate = new List<IRequirement>();
            void RecursiveAdd(
                ref IList<IRequirement> addTo,
                IReadOnlyList<IRequirement> remaining)
            {
                if (!remaining.Any())
                {
                    return;
                }

                foreach (IRequirement toCheck in remaining)
                {
                    if (!toCheck.DependsOn.Any() || toCheck.DependsOn.All(x => toEvaluate.Contains(x)))
                    {
                        toEvaluate.Add(toCheck);
                    }
                }
            }

            RecursiveAdd(ref toEvaluate, backingDictionary.Keys.ToList());

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            Exception ValidateRequirement(
                IRequirement requirement,
                object value,
                IConfiguration dependencies)
            {
                if (requirement.Type.Type.IsValueType
                    && !requirement.CollectionInfo.HasValue
                    && value == null)
                {
                    return new ArgumentNullException(
                        nameof(value),
                        Configuration.MappedValueNullButRequirementNotNullable);
                }

                if (requirement.CollectionInfo.HasValue
                    && value != null)
                {
                    if (value is IEnumerable asEnumerable)
                    {
                        ISetValidator validator = requirement.CreateSetValidator();
                        try
                        {
                            validator.Validate(asEnumerable);
                        }
                        catch (Exception e)
                        {
                            return e;
                        }
                    }
                    else
                    {
                        return new ArgumentException(
                            nameof(value),
                            Configuration.MappedValueIsNotEnumerableButRequirementHasEnumerableInfo);
                    }
                }
                else if (!requirement.CollectionInfo.HasValue)
                {
                    // TODO: Does this work? It's trying to see if `value` is in `RestrictedToSet`.
                    ISetValidator validator = requirement.CreateSetValidator();
                    try
                    {
                        validator.Validate(new[] { value });
                    }
                    catch (Exception e)
                    {
                        return e;
                    }
                }

                try
                {
                    return requirement.Validate(value, dependencies);
                }
                catch (Exception e)
                {
                    // Calling .Validate() shouldn't throw an exception (as per the contract), but just in case, treat
                    // thrown exceptions as failures.
                    return e;
                }
            }
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            // We can now iterate through toEvaluate; at each requirement, the set of previously completed requirements
            // will satisfy all dependsOns.
            Dictionary<IRequirement, object> completed =
                new Dictionary<IRequirement, object>();
            foreach (IRequirement requirement in toEvaluate)
            {
                object value = mappings[requirement];

                Exception failure = ValidateRequirement(
                    requirement,
                    value,
                    new DictionaryConfigurationAdapter(completed
                        .Where(x => requirement.DependsOn.Contains(x.Key))
                        .ToDictionary(x => x.Key, x => x.Value)));

                if (failure == null)
                {
                    completed.Add(requirement, value);
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

            this.mappingDictionary = new ReadOnlyDictionary<IRequirement, object>(completed);
        }

        public object this[IRequirement key] => this.mappingDictionary[key];

        public IEnumerable<IRequirement> Keys => this.mappingDictionary.Keys;

        public IEnumerable<object> Values => this.mappingDictionary.Values;

        public int Count => this.mappingDictionary.Count;

        public bool ContainsKey(IRequirement key) => this.mappingDictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<IRequirement, object>> GetEnumerator() =>
            this.mappingDictionary.GetEnumerator();

        public bool TryGetValue(IRequirement key, out object value) =>
            this.mappingDictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private sealed class DictionaryConfigurationAdapter : IConfiguration
        {
            private readonly IReadOnlyDictionary<IRequirement, object> backingDictionary;

            public DictionaryConfigurationAdapter(IDictionary<IRequirement, object> mappings)
            {
                this.backingDictionary = new ReadOnlyDictionary<IRequirement, object>(mappings);
            }

            public IEnumerable<IRequirement> Keys => this.backingDictionary.Keys;

            public IEnumerable<object> Values => this.backingDictionary.Values;

            public int Count => this.backingDictionary.Count;

            public object this[IRequirement key] => this.backingDictionary[key];

            public bool ContainsKey(IRequirement key) => this.backingDictionary.ContainsKey(key);

            public IEnumerator<KeyValuePair<IRequirement, object>> GetEnumerator() =>
                this.backingDictionary.GetEnumerator();

            public bool TryGetValue(IRequirement key, out object value) =>
                this.backingDictionary.TryGetValue(key, out value);

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}
