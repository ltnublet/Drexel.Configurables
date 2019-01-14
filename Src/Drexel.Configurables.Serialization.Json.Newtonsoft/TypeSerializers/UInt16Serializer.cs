using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public sealed class UInt16Serializer : SimpleStructTypeSerializer<UInt16>
    {
        public UInt16Serializer(StructRequirementType<UInt16> type)
            : base(type)
        {
        }

        public override Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            UInt16 value,
            CancellationToken cancellationToken = default)
        {
            return intermediary.Writer.WriteValueAsync(value, cancellationToken);
        }
    }
}
