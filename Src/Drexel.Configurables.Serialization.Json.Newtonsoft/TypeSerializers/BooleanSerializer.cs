using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public sealed class BooleanSerializer : SimpleStructTypeSerializer<Boolean>
    {
        public BooleanSerializer(StructRequirementType<Boolean> type)
            : base(type)
        {
        }

        public override Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            bool value,
            CancellationToken cancellationToken = default)
        {
            return intermediary.Writer.WriteValueAsync(value, cancellationToken);
        }
    }
}
