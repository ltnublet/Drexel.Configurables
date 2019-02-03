using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.FormattableString;

namespace Drexel.Configurables.Contracts.Ids
{
    public sealed class RequirementIdMapBuilder
    {
        private readonly object operationLock;
        private readonly Dictionary<Requirement, RequirementId> requirementToId;
        private readonly Dictionary<RequirementId, Requirement> idToRequirement;

        public RequirementIdMapBuilder()
        {
            this.operationLock = new object();
            this.requirementToId = new Dictionary<Requirement, RequirementId>();
            this.idToRequirement = new Dictionary<RequirementId, Requirement>();
        }

        public int Count => this.requirementToId.Count;

        public RequirementIdMapBuilder Add(Requirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            lock (this.operationLock)
            {
                RequirementId newId = RequirementId.NewRequirementId(idToRequirement.ContainsKey);

                this.AddInternal(requirement, newId);
            }

            return this;
        }

        public RequirementIdMapBuilder Add(Requirement requirement, Guid id)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            RequirementId newId = new RequirementId(id);

            lock (this.operationLock)
            {
                this.AddInternal(requirement, newId);
            }

            return this;
        }

        public RequirementIdMapBuilder Add(Requirement requirement, RequirementId id)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            lock (this.operationLock)
            {
                this.AddInternal(requirement, id);
            }

            return this;
        }

        public RequirementIdMapBuilder Add(RequirementIdMapBuilder requirementIdMapBuilder)
        {
            if (requirementIdMapBuilder == null)
            {
                throw new ArgumentNullException(nameof(requirementIdMapBuilder));
            }

            lock (this.operationLock)
            lock (requirementIdMapBuilder.operationLock)
            {
                foreach (KeyValuePair<Requirement, RequirementId> pair in requirementIdMapBuilder.requirementToId)
                {
                    this.AddInternal(pair.Key, pair.Value);
                }
            }

            return this;
        }

        public RequirementIdMapBuilder Add(IEnumerable<KeyValuePair<Requirement, RequirementId>> requirements)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            lock (this.operationLock)
            {
                foreach (KeyValuePair<Requirement, RequirementId> pair in requirements)
                {
                    this.AddInternal(pair.Key, pair.Value);
                }
            }

            return this;
        }

        public void Clear()
        {
            lock (this.operationLock)
            {
                this.requirementToId.Clear();
                this.idToRequirement.Clear();
            }
        }

        public RequirementIdMap Build()
        {
            lock (this.operationLock)
            {
                return new RequirementIdMap(
                    this.requirementToId,
                    this.idToRequirement);
            }
        }

        [DebuggerHidden]
        private void AddInternal(Requirement requirement, RequirementId id)
        {
            if (requirementToId.ContainsKey(requirement))
            {
                throw new InvalidOperationException(Invariant($"Duplicate {nameof(requirement)}."));
            }

            if (idToRequirement.ContainsKey(id))
            {
                throw new InvalidOperationException(Invariant($"Duplicate {nameof(id)}."));
            }

            requirementToId.Add(requirement, id);
            idToRequirement.Add(id, requirement);
        }
    }
}
