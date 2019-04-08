#nullable enable
using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementRelationsBuilder
    {
        private readonly object operationLock;
        private readonly Dictionary<Requirement, TreeNode<Requirement>> treeRoots;
        private readonly Dictionary<Requirement, TreeNode<Requirement>> treeNodes;
        private readonly Dictionary<Requirement, HashSet<Requirement>> exclusivities;

        public RequirementRelationsBuilder()
        {
            this.operationLock = new object();
            this.treeRoots = new Dictionary<Requirement, TreeNode<Requirement>>();
            this.treeNodes = new Dictionary<Requirement, TreeNode<Requirement>>();
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
                    if (!treeNodes.TryGetValue(primary, out TreeNode<Requirement> primaryNode))
                    {

                    }

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

            }
        }
    }
}
