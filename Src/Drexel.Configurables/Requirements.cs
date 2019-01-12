using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;
using Drexel.Configurables.External;

namespace Drexel.Configurables
{
    public static class Requirements
    {
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

        public static StructRequirement<Boolean> CreateBoolean(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Boolean>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new StructRequirement<Boolean>(
                id,
                RequirementTypes.Boolean,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }

        public static StructRequirement<Decimal> CreateDecimal(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Decimal>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new StructRequirement<Decimal>(
                id,
                RequirementTypes.Decimal,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }

        public static StructRequirement<Double> CreateDouble(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Double>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new StructRequirement<Double>(
                id,
                RequirementTypes.Double,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }

        public static ClassRequirement<FilePath> CreateFilePath(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<ClassSetRestrictionInfo<FilePath>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new ClassRequirement<FilePath>(
                id,
                RequirementTypes.FilePath,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }

        public static StructRequirement<Int32> CreateInt32(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Int32>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new StructRequirement<Int32>(
                id,
                RequirementTypes.Int32,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }
        public static StructRequirement<Int64> CreateInt64(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Int64>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new StructRequirement<Int64>(
                id,
                RequirementTypes.Int64,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }
        public static ClassRequirement<SecureString> CreateSecureString(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<ClassSetRestrictionInfo<SecureString>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new ClassRequirement<SecureString>(
                id,
                RequirementTypes.SecureString,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }
        public static StructRequirement<Single> CreateSingle(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Single>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new StructRequirement<Single>(
                id,
                RequirementTypes.Single,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }
        public static ClassRequirement<String> CreateString(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<ClassSetRestrictionInfo<String>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new ClassRequirement<String>(
                id,
                RequirementTypes.String,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }

        public static StructRequirement<UInt16> CreateUInt16(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<StructSetRestrictionInfo<UInt16>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new StructRequirement<UInt16>(
                id,
                RequirementTypes.UInt16,
                isOptional,
                collectionInfo,
                restrictedToSet,
                relations,
                validationCallback);
        }

        public static StructRequirement<UInt64> CreateUInt64(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            IReadOnlyCollection<StructSetRestrictionInfo<UInt64>>? restrictedToSet = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null)
        {
            return new StructRequirement<UInt64>(
                id,
                RequirementTypes.UInt64,
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
