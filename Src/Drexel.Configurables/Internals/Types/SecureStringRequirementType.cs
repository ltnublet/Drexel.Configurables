using System;
using System.Security;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Internals.Types
{
    internal static class SecureStringRequirementType
    {
        private static readonly string Id = @"C6EB8E40-EFB7-4943-996F-EFE81E120491";

        public static ClassRequirementType<SecureString> Instance { get; } =
            new ClassRequirementType<SecureString>(
                Guid.Parse(SecureStringRequirementType.Id),
                DefaultMethods.TryCastClassValue,
                DefaultMethods.TryCastClassCollection);
    }
}
