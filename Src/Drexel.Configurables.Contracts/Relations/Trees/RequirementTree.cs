#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementTree : IReadOnlyList<RequirementTree>
    {
        internal RequirementTree(RequirementTreeNode root)
        {
            this.Root = root;
        }

        public RequirementTreeNode Root { get; }

        public RequirementTree this[int index]
        {
            get
            {
                if (index < 0 || index > this.Root.MutableChildren.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return new RequirementTree(this.Root.MutableChildren[index]);
            }
        }

        public int Count => this.Root.Children.Count;

        public IReadOnlyList<RequirementTreeNode> BreadthFirstSort()
        {
            HashSet<RequirementTreeNode> visited = new HashSet<RequirementTreeNode>();
            List<RequirementTreeNode> returnValue = new List<RequirementTreeNode>();

            Queue<RequirementTreeNode> queue = new Queue<RequirementTreeNode>();
            queue.Enqueue(this.Root);

            while (queue.Any())
            {
                RequirementTreeNode currentNode = queue.Dequeue();

                if (visited.Contains(currentNode))
                {
                    throw new InvalidOperationException("Circular dependency");
                }
                else
                {
                    returnValue.Add(currentNode);
                    visited.Add(currentNode);
                }

                foreach (RequirementTreeNode child in currentNode.Children)
                {
                    queue.Enqueue(child);
                }
            }

            return returnValue;
        }

        public IEnumerator<RequirementTree> GetEnumerator() =>
            this.Root.Children.Select(x => new RequirementTree(x)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
