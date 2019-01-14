using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public sealed class UriSerializer : SimpleClassTypeSerializer<Uri>
    {
        public UriSerializer(ClassRequirementType<Uri> type)
            : base(type)
        {
        }

        public override Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            Uri value,
            CancellationToken cancellationToken = default)
        {
            return intermediary.Writer.WriteValueAsync(value, cancellationToken);
        }
    }
}
