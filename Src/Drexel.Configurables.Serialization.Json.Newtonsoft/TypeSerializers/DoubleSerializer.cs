using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public sealed class DoubleSerializer : SimpleStructTypeSerializer<Double>
    {
        public DoubleSerializer(StructRequirementType<Double> type)
            : base(type)
        {
        }

        public override Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            Double value,
            CancellationToken cancellationToken = default)
        {
            return intermediary.Writer.WriteValueAsync(value, cancellationToken);
        }
    }
}
