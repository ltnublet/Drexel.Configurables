namespace Drexel.Configurables.Contracts.Classes
{
    public sealed class ClassValueRequirement<T> : ClassRequirement<T>
        where T : class
    {
        public ClassValueRequirement(ClassRequirementType<T> type)
            : base(type)
        {
        }
    }
}
