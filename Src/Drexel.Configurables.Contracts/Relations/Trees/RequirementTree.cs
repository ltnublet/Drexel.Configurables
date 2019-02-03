using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementTree : IReadOnlyList<RequirementTree>
    {
        internal RequirementTree(RequirementTreeNode root)
        {
            this.Root = root;
        }

        public RequirementTreeNode Root { get; }

        public RequirementTree this[int index] => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public IReadOnlyList<RequirementTreeNode> TopologicalSort()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<RequirementTree> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
