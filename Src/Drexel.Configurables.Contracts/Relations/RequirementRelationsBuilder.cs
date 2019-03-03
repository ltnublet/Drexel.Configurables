#nullable enable
using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementRelationsBuilder
    {
        private readonly object operationLock;
        private readonly Dictionary<Requirement, Dictionary<Requirement, RequirementRelation>> rawRelations;
        private readonly Dictionary<Requirement, HashSet<Requirement>> dependencies;
        private readonly Dictionary<Requirement, HashSet<Requirement>> exclusivities;

        public RequirementRelationsBuilder()
        {
            this.operationLock = new object();
            this.rawRelations = new Dictionary<Requirement, Dictionary<Requirement, RequirementRelation>>();
            this.dependencies = new Dictionary<Requirement, HashSet<Requirement>>();
            this.exclusivities = new Dictionary<Requirement, HashSet<Requirement>>();
        }

        public RequirementRelationsBuilder Add(
            Requirement primary,
            Requirement secondary,
            RequirementRelation relation)
        {
            if (primary == null)
            {
                throw new ArgumentNullException(nameof(primary));
            }

            if (secondary == null)
            {
                throw new ArgumentNullException(nameof(secondary));
            }

            if (relation != RequirementRelation.None)
            {
                lock (this.operationLock)
                {
                    if (!this.rawRelations.TryGetValue(primary, out Dictionary<Requirement, RequirementRelation> raw))
                    {
                        raw = new Dictionary<Requirement, RequirementRelation>();
                        this.rawRelations.Add(primary, raw);
                    }
                    else
                    {
                        if (raw.ContainsKey(secondary))
                        {
                            throw new InvalidOperationException(
                                "A relationship already exists between the specified requirements.");
                        }
                    }

                    raw.Add(secondary, relation);

                    Requirement parent = primary;
                    Requirement child = secondary;
                    switch (relation)
                    {
                        case RequirementRelation.DependsOn:
                            parent = secondary;
                            child = primary;
                            relation = RequirementRelation.DependedUpon;
                            goto case RequirementRelation.DependedUpon;
                        case RequirementRelation.DependedUpon:
                            if (!this.dependencies.TryGetValue(parent, out HashSet<Requirement> parentDependents))
                            {
                                parentDependents = new HashSet<Requirement>();
                                this.dependencies.Add(parent, parentDependents);
                            }

                            parentDependents.Add(child);
                            break;
                        case RequirementRelation.ExclusiveWith:
                            if (!this.exclusivities.TryGetValue(parent, out HashSet<Requirement> parentExclusives))
                            {
                                parentExclusives = new HashSet<Requirement>();
                                this.exclusivities.Add(parent, parentExclusives);
                            }

                            if (!this.exclusivities.TryGetValue(child, out HashSet<Requirement> childExclusives))
                            {
                                childExclusives = new HashSet<Requirement>();
                                this.exclusivities.Add(child, childExclusives);
                            }

                            parentExclusives.Add(child);
                            childExclusives.Add(parent);
                            break;
                    }
                }
            }

            return this;
        }

        public RequirementRelations Build()
        {
            lock (this.operationLock)
            {
                return new RequirementRelations(
                    this.rawRelations,
                    this.dependencies,
                    this.exclusivities);
            }
        }

        public void Clear()
        {
            lock (this.operationLock)
            {
                this.rawRelations.Clear();
                this.dependencies.Clear();
                this.exclusivities.Clear();
            }
        }
    }
}
