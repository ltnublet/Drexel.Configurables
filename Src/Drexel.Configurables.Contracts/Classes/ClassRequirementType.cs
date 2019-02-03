namespace Drexel.Configurables.Contracts.Classes
{
    public abstract class ClassRequirementType<T> : RequirementType
        where T : class
    {
        private protected ClassRequirementType()
            : base(typeof(T))
        {
        }
    }
}
