using System;

namespace Drexel.Configurables.Contracts.Relations
{
    internal static class RequirementRelationExtensions
    {
        public static RequirementRelation Inverse(this RequirementRelation relation) =>
            relation switch
            {
                RequirementRelation.DependedUpon => RequirementRelation.DependsOn,
                RequirementRelation.DependsOn => RequirementRelation.DependedUpon,
                RequirementRelation.ExclusiveWith => RequirementRelation.ExclusiveWith,
                RequirementRelation.None => RequirementRelation.None,
                _ => throw new NotImplementedException()
            };

        public static bool IsDependency(this RequirementRelation relation) =>
            relation switch
            {
                RequirementRelation.DependedUpon => true,
                RequirementRelation.DependsOn => true,
                _ => false
            };
    }
}
