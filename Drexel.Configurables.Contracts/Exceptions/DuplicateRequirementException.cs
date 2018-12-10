using System;
using System.Collections.Generic;
using System.Text;

namespace Drexel.Configurables.Contracts.Exceptions
{
    public class DuplicateRequirementException : RequirementCollectionException
    {
        public DuplicateRequirementException(IRequirement requirement)
            : base("Specified requirement collection contains duplicate requirement.")
        {
            this.Duplicate = requirement;
        }

        public IRequirement Duplicate { get; }
    }
}
