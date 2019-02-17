#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.FormattableString;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementRelationsBuilder
    {
        private readonly object operationLock;
        private Dictionary<Requirement, Dictionary<Requirement, RequirementRelation>> mappings;
        private Dictionary<RequirementTree, Dictionary<Requirement, RequirementTreeNode>> treeNodes;

        public RequirementRelationsBuilder()
        {
            this.operationLock = new object();
            this.mappings = new Dictionary<Requirement, Dictionary<Requirement, RequirementRelation>>();
            this.treeNodes = new Dictionary<RequirementTree, Dictionary<Requirement, RequirementTreeNode>>();
        }

        public int Count => this.mappings.Count;

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

            RequirementRelationsBuilder.ThrowIfIllegalRelation(relation);

            lock (this.operationLock)
            {
                RequirementRelationsBuilder.AddInternal(
                    ref this.mappings,
                    ref this.treeNodes,
                    primary,
                    secondary,
                    relation);
            }

            return this;
        }

        public RequirementRelationsBuilder Add(
            Requirement primary,
            IEnumerable<KeyValuePair<Requirement, RequirementRelation>> relations)
        {
            if (primary == null)
            {
                throw new ArgumentNullException(nameof(primary));
            }

            if (relations == null)
            {
                throw new ArgumentNullException(nameof(relations));
            }

            lock (this.operationLock)
            {
                foreach (KeyValuePair<Requirement, RequirementRelation> relation in relations)
                {
                    if (relation.Key == null)
                    {
                        throw new ArgumentException(
                            Invariant($"Specified relations set contains null {nameof(Requirement)}."),
                            nameof(relations));
                    }

                    RequirementRelationsBuilder.ThrowIfIllegalRelation(relation.Value);

                    RequirementRelationsBuilder.AddInternal(
                        ref this.mappings,
                        ref this.treeNodes,
                        primary,
                        relation.Key,
                        relation.Value);
                }
            }

            return this;
        }

        public RequirementRelationsBuilder Add(RequirementRelationsBuilder requirementRelationsBuilder)
        {
            if (requirementRelationsBuilder == null)
            {
                throw new ArgumentNullException(nameof(requirementRelationsBuilder));
            }

            lock (this.operationLock)
            lock (requirementRelationsBuilder.operationLock)
            {
                foreach (KeyValuePair<Requirement, Dictionary<Requirement, RequirementRelation>> primary in
                    requirementRelationsBuilder.mappings)
                {
                    foreach (KeyValuePair<Requirement, RequirementRelation> secondary in primary.Value)
                    {
                        RequirementRelationsBuilder.AddInternal(
                            ref this.mappings,
                            ref this.treeNodes,
                            primary.Key,
                            secondary.Key,
                            secondary.Value);
                    }
                }
            }

            return this;
        }

        public RequirementRelations Build()
        {
            lock (this.operationLock)
            {
                return new RequirementRelations(this.mappings, this.treeNodes.Keys.ToList());
            }
        }

        public void Clear()
        {
            lock (this.operationLock)
            {
                this.mappings.Clear();
                this.treeNodes.Clear();
            }
        }

        internal static void AddInternal(
            ref Dictionary<Requirement, Dictionary<Requirement, RequirementRelation>> mappings,
            ref Dictionary<RequirementTree, Dictionary<Requirement, RequirementTreeNode>> treeNodes,
            Requirement primary,
            Requirement secondary,
            RequirementRelation relation)
        {
            RequirementRelationsBuilder.AddMappingsInternal(
                ref mappings,
                primary,
                secondary,
                relation);

            RequirementRelationsBuilder.AddTreesInternal(
                ref treeNodes,
                primary,
                secondary,
                relation);
        }

        internal static void AddMappingsInternal(
            ref Dictionary<Requirement, Dictionary<Requirement, RequirementRelation>> mappings,
            Requirement primary,
            Requirement secondary,
            RequirementRelation relation)
        {
            if (mappings.TryGetValue(
                primary,
                out Dictionary<Requirement, RequirementRelation> relations))
            {
                if (relations.ContainsKey(secondary))
                {
                    throw new InvalidOperationException(
                        Invariant(
                            $"A relationship between the specified {nameof(primary)} and {nameof(secondary)} already exists."));
                }
            }
            else
            {
                relations = new Dictionary<Requirement, RequirementRelation>();
                mappings.Add(primary, relations);
            }

            relations.Add(secondary, relation);

            if (!mappings.TryGetValue(
                secondary,
                out Dictionary<Requirement, RequirementRelation> inverseRelations))
            {
                inverseRelations = new Dictionary<Requirement, RequirementRelation>();
                mappings[secondary] = inverseRelations;
            }

            inverseRelations.Add(primary, relation.Inverse());
        }

        internal static void AddTreesInternal(
            ref Dictionary<RequirementTree, Dictionary<Requirement, RequirementTreeNode>> treeNodes,
            Requirement primary,
            Requirement secondary,
            RequirementRelation relation)
        {
            Requirement parent;
            Requirement child;
            switch (relation)
            {
                case RequirementRelation.DependedUpon:
                    parent = primary;
                    child = secondary;
                    break;
                case RequirementRelation.DependsOn:
                    parent = secondary;
                    child = primary;
                    break;
                default:
                    // The dependency trees consist only of dependencies, and don't include exclusiveness.
                    return;
            }

            // If treeNodes contains a tree that contains the parent,
            //   If the same tree contains the child,
            // Else,
            //   

            foreach (KeyValuePair<RequirementTree, Dictionary<Requirement, RequirementTreeNode>> trees in treeNodes)
            {
                if (trees.Value.TryGetValue(parent, out RequirementTreeNode parentNode))
                {
                    if (!trees.Value.TryGetValue(child, out RequirementTreeNode childNode))
                    {
                        childNode = new RequirementTreeNode(child);
                        trees.Value.Add(child, childNode);
                    }

                    childNode.MutableParents.Add(parentNode);
                    parentNode.MutableChildren.Add(childNode);

                    break;
                }
                else
                {

                }
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThrowIfIllegalRelation(RequirementRelation relation)
        {
            switch (relation)
            {
                case RequirementRelation.None:
                    throw new ArgumentException(
                        Invariant($"Cannot add a relation of type {RequirementRelation.None}."));
            }
        }
    }
}
