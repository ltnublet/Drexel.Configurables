using System;

namespace Drexel.Configurables.Contracts.Ids
{
    public sealed class RequirementId : IEquatable<RequirementId>
    {
        private readonly Guid id;

        internal RequirementId(Guid id)
        {
            this.id = id;
        }

        internal static RequirementId NewRequirementId()
        {
            return new RequirementId(Guid.NewGuid());
        }

        internal static RequirementId NewRequirementId(Func<RequirementId, bool> isInvalid)
        {
            RequirementId id;
            do
            {
                id = RequirementId.NewRequirementId();
            }
            while (isInvalid.Invoke(id));

            return id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is RequirementId asRequirementId)
            {
                return this.Equals(asRequirementId);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(RequirementId other)
        {
            if (other == null)
            {
                return false;
            }

            return this.id.Equals(other.id);
        }
    }
}
