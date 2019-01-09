using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents the relationships between a <see cref="Requirement"/> and other <see cref="Requirement"/>s, such
    /// as exclusivity or dependency.
    /// </summary>
    public sealed class RequirementRelations
    {
        internal RequirementRelations(
            IReadOnlyCollection<Requirement>? dependsOn = null,
            IReadOnlyCollection<Requirement>? exclusiveWith = null)
        {
            this.DependsOn = new ReadOnlyCollection<Requirement>(
                dependsOn?.ToList() ?? Array.Empty<Requirement>().ToList());
            this.ExclusiveWith = new ReadOnlyCollection<Requirement>(
                exclusiveWith?.ToList() ?? Array.Empty<Requirement>().ToList());
        }

        /// <summary>
        /// Gets the set of requirements that this requirement depends on.
        /// </summary>
        public IReadOnlyCollection<Requirement> DependsOn { get; }

        /// <summary>
        /// Gets the set of requirements that this requirement is exclusive with.
        /// </summary>
        public IReadOnlyCollection<Requirement> ExclusiveWith { get; }
    }
}
