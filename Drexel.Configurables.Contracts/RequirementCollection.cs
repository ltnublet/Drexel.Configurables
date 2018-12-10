using System;
using System.Collections;
using System.Collections.Generic;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables.Contracts
{
    public sealed class RequirementCollection : IReadOnlyCollection<IRequirement>
    {
        public RequirementCollection(IReadOnlyCollection<IRequirement> requirements)
        {
            HashSet<IRequirement> distinctRequirements = new HashSet<IRequirement>();
            foreach (IRequirement requirement in requirements)
            {
                if (distinctRequirements.Contains(requirement))
                {
                    throw new DuplicateRequirementException(requirement);
                }
            }
        }

        public int Count => throw new NotImplementedException();

        public IEnumerator<IRequirement> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
