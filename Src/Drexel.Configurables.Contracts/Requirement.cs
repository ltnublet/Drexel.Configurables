using System;
using System.Diagnostics;

namespace Drexel.Configurables.Contracts
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public abstract class Requirement
    {
        private protected Requirement(RequirementType type)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));

            this.DebuggerDisplay = Guid.NewGuid();
        }

        public RequirementType Type { get; }

        private Guid DebuggerDisplay { get; }
    }
}
