using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public sealed class SecureStringSerializer : BaseClassTypeSerializer<SecureString>
    {
        public SecureStringSerializer(ClassRequirementType<SecureString> type)
            : base(type)
        {
        }

        public override bool SupportsWrites => false;

        public override Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            SecureString value,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            IEnumerable<SecureString> values,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
