using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementForest : IReadOnlyList<RequirementTree>
    {
        internal RequirementForest(RequirementRelations relations)
        {
            Dictionary<Requirement, RequirementTree> trees =
                new Dictionary<Requirement, RequirementTree>();

            // 1. For all requirements which have no dependencies, create a new tree with them as the root.
            foreach (Requirement key in relations
                .Where(
                    x =>
                    {
                        if (x.Value.Count == 0)
                        {
                            return true;
                        }
                        else if (x.Value.All(y => !y.Value.IsDependency()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    })
                .Select(x => x.Key))
            {
                RequirementTreeNode asNode = new RequirementTreeNode(key);
                trees[key] = new RequirementTree(asNode);
            }
        }

        public RequirementTree this[int index] => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

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
