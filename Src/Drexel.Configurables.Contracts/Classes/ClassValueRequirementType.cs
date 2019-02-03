namespace Drexel.Configurables.Contracts.Classes
{
    public sealed class ClassValueRequirementType<T> : ClassRequirementType<T>
        where T : class
    {
        public ClassValueRequirementType()
            : base()
        {
        }
    }
}
