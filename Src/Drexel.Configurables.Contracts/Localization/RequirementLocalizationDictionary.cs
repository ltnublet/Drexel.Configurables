#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Localization
{
    public sealed class RequirementLocalizationDictionary : IReadOnlyDictionary<Requirement, RequirementLocalization>
    {
        private readonly Dictionary<Requirement, RequirementLocalization> localizations;

        internal RequirementLocalizationDictionary(
            IReadOnlyCollection<Requirement> requirements,
            IReadOnlyDictionary<Requirement, string> names,
            IReadOnlyDictionary<Requirement, string> descriptions)
        {
            this.localizations = new Dictionary<Requirement, RequirementLocalization>(requirements.Count);

            foreach (Requirement requirement in requirements)
            {
#pragma warning disable CS8620 // Nullability of reference types in argument doesn't match target type.
                string? name = names.GetOrDefault(requirement, (string?)null);
                string? description = descriptions.GetOrDefault(requirement, (string?)null);
#pragma warning restore CS8620 // Nullability of reference types in argument doesn't match target type.

                this.localizations.Add(requirement, new RequirementLocalization(name, description));
            }
        }

        public RequirementLocalization this[Requirement key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (this.localizations.TryGetValue(key, out RequirementLocalization value))
                {
                    return value;
                }

                throw new KeyNotFoundException();
            }
        }

        public IReadOnlyCollection<Requirement> Keys => this.localizations.Keys;

        public IReadOnlyCollection<RequirementLocalization> Values => this.localizations.Values;

        public int Count => this.localizations.Count;

        public bool ContainsKey(Requirement key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return this.localizations.ContainsKey(key);
        }

        public bool TryGetValue(Requirement key, out RequirementLocalization value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return this.localizations.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<Requirement, RequirementLocalization>> GetEnumerator() =>
            this.localizations.GetEnumerator();

        IEnumerable<Requirement> IReadOnlyDictionary<Requirement, RequirementLocalization>.Keys =>
            this.Keys;

        IEnumerable<RequirementLocalization> IReadOnlyDictionary<Requirement, RequirementLocalization>.Values =>
            this.Values;

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
