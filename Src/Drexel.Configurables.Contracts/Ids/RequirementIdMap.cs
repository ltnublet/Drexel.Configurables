#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Drexel.Configurables.Contracts.Ids
{
    public sealed class RequirementIdMap
    {
        internal RequirementIdMap(
            IDictionary<Requirement, RequirementId> requirementToId,
            IDictionary<RequirementId, Requirement> idToRequirement)
        {
            this.RequirementToId = new ReadOnlyDictionary<Requirement, RequirementId>(
                requirementToId ?? throw new ArgumentNullException(nameof(requirementToId)));
            this.IdToRequirement = new ReadOnlyDictionary<RequirementId, Requirement>(
                idToRequirement ?? throw new ArgumentNullException(nameof(idToRequirement)));
        }

        public IReadOnlyDictionary<Requirement, RequirementId> RequirementToId { get; }

        public IReadOnlyDictionary<RequirementId, Requirement> IdToRequirement { get; }
    }
}
