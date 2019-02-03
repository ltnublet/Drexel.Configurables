namespace Drexel.Configurables.Contracts.Tests.Common.Mocks
{
    public class MockRequirement : Requirement
    {
        public static MockRequirement Instance { get; } = new MockRequirement();

        public MockRequirement()
            : this(MockRequirementType.Instance)
        {
        }

        public MockRequirement(RequirementType type)
            : base(type)
        {
        }
    }
}
