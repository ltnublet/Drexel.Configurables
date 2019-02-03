namespace Drexel.Configurables.Contracts.Structs
{
    public sealed class StructValueRequirement<T> : StructRequirement<T>
        where T : struct
    {
        public StructValueRequirement(StructRequirementType<T> type)
            : base(type)
        {
        }
    }
}
