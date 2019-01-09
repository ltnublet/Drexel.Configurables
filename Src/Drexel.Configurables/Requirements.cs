using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables
{
    public static class Requirements
    {
        ////Types.Boolean = BooleanRequirementType.Instance;
        ////Types.Decimal = DecimalRequirementType.Instance;
        ////Types.Double = DoubleRequirementType.Instance;
        ////Types.FilePath = FilePathRequirementType.Instance;
        ////Types.Int32 = Int32RequirementType.Instance;
        ////Types.Int64 = Int64RequirementType.Instance;
        ////Types.SecureString = SecureStringRequirementType.Instance;
        ////Types.Single = SingleRequirementType.Instance;
        ////Types.UInt64 = UInt64RequirementType.Instance;

        public static StructRequirement<BigInteger> CreateBigInteger(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<StructSetRestrictionInfo<BigInteger>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new StructRequirement<BigInteger>(
                id,
                RequirementTypes.BigInteger,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }

        public static ClassRequirement<Uri> CreateUri(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<ClassSetRestrictionInfo<Uri>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new ClassRequirement<Uri>(
                id,
                RequirementTypes.Uri,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }
    }
}
