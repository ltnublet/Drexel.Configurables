using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Localization
{
    public sealed class RequirementLocalizationDictionaryBuilder
    {
        private readonly HashSet<Requirement> uniqueRequirements;
        private readonly Dictionary<Requirement, string> names;
        private readonly Dictionary<Requirement, string> descriptions;
        private readonly object operationLock;

        public RequirementLocalizationDictionaryBuilder()
        {
            this.uniqueRequirements = new HashSet<Requirement>();
            this.names = new Dictionary<Requirement, string>();
            this.descriptions = new Dictionary<Requirement, string>();
            this.operationLock = new object();
        }

        public int Count => this.uniqueRequirements.Count;

        public RequirementLocalizationDictionaryBuilder AddName(Requirement requirement, string name)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            lock (this.operationLock)
            {
                this.uniqueRequirements.Add(requirement);
                this.names.Add(requirement, name);
            }

            return this;
        }

        public RequirementLocalizationDictionaryBuilder AddDescription(Requirement requirement, string description)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            lock (this.operationLock)
            {
                this.uniqueRequirements.Add(requirement);
                this.descriptions.Add(requirement, description);
            }

            return this;
        }

        public RequirementLocalizationDictionary Build()
        {
            lock (this.operationLock)
            {
                return new RequirementLocalizationDictionary(
                    this.uniqueRequirements,
                    this.names,
                    this.descriptions);
            }
        }

        public void Clear()
        {
            lock (this.operationLock)
            {
                this.uniqueRequirements.Clear();
                this.names.Clear();
                this.descriptions.Clear();
            }
        }
    }
}
