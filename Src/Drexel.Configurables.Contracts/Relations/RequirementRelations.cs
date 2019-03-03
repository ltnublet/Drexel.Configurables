#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementRelations
    {
        private readonly IReadOnlyDictionary<Requirement, IReadOnlyDictionary<Requirement, RequirementRelation>> rawRelations;
        private readonly IReadOnlyDictionary<Requirement, HashSet<Requirement>> dependencies;
        private readonly IReadOnlyDictionary<Requirement, HashSet<Requirement>> exclusivities;

        internal RequirementRelations(
            IReadOnlyDictionary<Requirement, Dictionary<Requirement, RequirementRelation>> rawRelations,
            IReadOnlyDictionary<Requirement, HashSet<Requirement>> dependencies,
            IReadOnlyDictionary<Requirement, HashSet<Requirement>> exclusivities)
        {
            this.rawRelations = rawRelations
                ;
            this.dependencies = dependencies;
            this.exclusivities = exclusivities;
        }

        public bool TryGetRelations(
            Requirement requirement,
            out IReadOnlyDictionary<Requirement, RequirementRelation> relations)
        {

        }

        public IEnumerable<Requirement> BreadthFirstSort()
        {

        } 
    }
}
