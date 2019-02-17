#nullable enable
using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementTreeNode
    {
        internal RequirementTreeNode(Requirement value)
            : this(value, Array.Empty<RequirementTreeNode>())
        {
        }

        internal RequirementTreeNode(Requirement value, IEnumerable<RequirementTreeNode> parents)
            : this(value, parents, Array.Empty<RequirementTreeNode>())
        {
        }

        internal RequirementTreeNode(
            Requirement value,
            IEnumerable<RequirementTreeNode> parents,
            IEnumerable<RequirementTreeNode> children)
        {
            this.Value = value;

            HashSet<RequirementTreeNode> childBuffer = new HashSet<RequirementTreeNode>(children);
            HashSet<RequirementTreeNode> parentBuffer = new HashSet<RequirementTreeNode>(parents);
            this.MutableChildren = childBuffer;
            this.Children = childBuffer;
            this.MutableParents = parentBuffer;
            this.Parents = parentBuffer;
        }

        public Requirement Value { get; internal set; }

        public IReadOnlyCollection<RequirementTreeNode> Parents{ get; internal set; }

        public IReadOnlyCollection<RequirementTreeNode> Children { get; }

        internal HashSet<RequirementTreeNode> MutableChildren { get; }

        internal HashSet<RequirementTreeNode> MutableParents { get; }
    }
}
