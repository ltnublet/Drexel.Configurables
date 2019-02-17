#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementForest : IReadOnlyList<RequirementTree>
    {
        internal RequirementForest(List<RequirementTree> trees)
        {
            this.Trees = trees;
        }

        public RequirementTree this[int index]
        {
            get
            {
                if (index < 0 || index > this.Trees.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return this.Trees[index];
            }
        }

        public int Count => this.Trees.Count;

        internal List<RequirementTree> Trees { get; }

        public IEnumerator<RequirementTree> GetEnumerator() => this.Trees.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
