using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementTreeNode
    {
        internal RequirementTreeNode(Requirement value)
            : this(value, null)
        {
        }

        internal RequirementTreeNode(Requirement value, RequirementTreeNode parent)
            : this(value, parent, Array.Empty<RequirementTreeNode>())
        {
        }

        internal RequirementTreeNode(
            Requirement value,
            RequirementTreeNode parent,
            IEnumerable<RequirementTreeNode> children)
        {

            this.Value = value;
            this.Parent = parent;

            List<RequirementTreeNode> buffer = new List<RequirementTreeNode>(children);
            this.MutableChildren = buffer;
            this.Children = buffer;
        }

#nullable enable
        public RequirementTreeNode? Parent { get; internal set; }
#nullable disable

        public Requirement Value { get; internal set; }

        public IReadOnlyCollection<RequirementTreeNode> Children { get; }

        internal IList<RequirementTreeNode> MutableChildren { get; }
    }
}
