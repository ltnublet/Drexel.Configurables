namespace Drexel.Configurables.Contracts.Structs
{
    public sealed class StructValueRequirementType<T> : StructRequirementType<T>
        where T : struct
    {
        public StructValueRequirementType()
            : base()
        {
        }
    }
}
