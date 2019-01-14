using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public sealed class DateTimeSerializer : SimpleStructTypeSerializer<DateTime>
    {
        public DateTimeSerializer(StructRequirementType<DateTime> type)
            : base(type)
        {
        }

        public override Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            DateTime value,
            CancellationToken cancellationToken = default)
        {
            return intermediary.Writer.WriteValueAsync(value, cancellationToken);
        }
    }
}
