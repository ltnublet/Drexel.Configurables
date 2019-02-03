namespace Drexel.Configurables.Contracts.Structs
{
    public abstract class StructRequirementType<T> : RequirementType
        where T : struct
    {
        private protected StructRequirementType()
            : base(typeof(T))
        {
        }
    }
}
