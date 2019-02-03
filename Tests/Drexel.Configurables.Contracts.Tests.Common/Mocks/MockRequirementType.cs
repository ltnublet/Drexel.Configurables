using System;

namespace Drexel.Configurables.Contracts.Tests.Common.Mocks
{
    public class MockRequirementType : RequirementType
    {
        public static MockRequirementType Instance { get; } = new MockRequirementType();

        public MockRequirementType()
            : this(typeof(MockType))
        {
        }

        public MockRequirementType(Type type)
            : base(type)
        {
        }
    }
}
