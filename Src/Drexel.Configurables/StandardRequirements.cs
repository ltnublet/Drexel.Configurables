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
    public static class StandardRequirements
    {
        public static StructRequirement<BigInteger> CreateBigInteger(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<BigInteger>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<BigInteger>>? restrictedToSet = null)
        {
            return new StructRequirement<BigInteger>(
                id,
                StandardRequirementTypes.BigInteger,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<Boolean> CreateBoolean(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<Boolean>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Boolean>>? restrictedToSet = null)
        {
            return new StructRequirement<Boolean>(
                id,
                StandardRequirementTypes.Boolean,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<Decimal> CreateDecimal(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<Decimal>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Decimal>>? restrictedToSet = null)
        {
            return new StructRequirement<Decimal>(
                id,
                StandardRequirementTypes.Decimal,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<DateTime> CreateDateTime(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<DateTime>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<DateTime>>? restrictedToSet = null)
        {
            return new StructRequirement<DateTime>(
                id,
                StandardRequirementTypes.DateTime,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<Double> CreateDouble(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<Double>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Double>>? restrictedToSet = null)
        {
            return new StructRequirement<Double>(
                id,
                StandardRequirementTypes.Double,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static ClassRequirement<FilePath> CreateFilePath(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            ClassDefaultValue<FilePath>? defaultValue = null,
            IReadOnlyCollection<ClassSetRestrictionInfo<FilePath>>? restrictedToSet = null)
        {
            return new ClassRequirement<FilePath>(
                id,
                StandardRequirementTypes.FilePath,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<Int32> CreateInt32(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<Int32>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Int32>>? restrictedToSet = null)
        {
            return new StructRequirement<Int32>(
                id,
                StandardRequirementTypes.Int32,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<Int64> CreateInt64(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<Int64>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Int64>>? restrictedToSet = null)
        {
            return new StructRequirement<Int64>(
                id,
                StandardRequirementTypes.Int64,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static ClassRequirement<SecureString> CreateSecureString(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            ClassDefaultValue<SecureString>? defaultValue = null,
            IReadOnlyCollection<ClassSetRestrictionInfo<SecureString>>? restrictedToSet = null)
        {
            return new ClassRequirement<SecureString>(
                id,
                StandardRequirementTypes.SecureString,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<Single> CreateSingle(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<Single>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<Single>>? restrictedToSet = null)
        {
            return new StructRequirement<Single>(
                id,
                StandardRequirementTypes.Single,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static ClassRequirement<String> CreateString(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            ClassDefaultValue<String>? defaultValue = null,
            IReadOnlyCollection<ClassSetRestrictionInfo<String>>? restrictedToSet = null)
        {
            return new ClassRequirement<String>(
                id,
                StandardRequirementTypes.String,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<TimeSpan> CreateTimeSpan(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<TimeSpan>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<TimeSpan>>? restrictedToSet = null)
        {
            return new StructRequirement<TimeSpan>(
                id,
                StandardRequirementTypes.TimeSpan,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<UInt16> CreateUInt16(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<UInt16>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<UInt16>>? restrictedToSet = null)
        {
            return new StructRequirement<UInt16>(
                id,
                StandardRequirementTypes.UInt16,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static StructRequirement<UInt64> CreateUInt64(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            StructDefaultValue<UInt64>? defaultValue = null,
            IReadOnlyCollection<StructSetRestrictionInfo<UInt64>>? restrictedToSet = null)
        {
            return new StructRequirement<UInt64>(
                id,
                StandardRequirementTypes.UInt64,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }

        public static ClassRequirement<Uri> CreateUri(
            Guid id,
            bool isOptional = false,
            CollectionInfo? collectionInfo = null,
            RequirementRelations? relations = null,
            Func<object?, Configuration, Task>? validationCallback = null,
            ClassDefaultValue<Uri>? defaultValue = null,
            IReadOnlyCollection<ClassSetRestrictionInfo<Uri>>? restrictedToSet = null)
        {
            return new ClassRequirement<Uri>(
                id,
                StandardRequirementTypes.Uri,
                isOptional,
                collectionInfo,
                relations,
                validationCallback,
                defaultValue,
                restrictedToSet);
        }
    }
}
