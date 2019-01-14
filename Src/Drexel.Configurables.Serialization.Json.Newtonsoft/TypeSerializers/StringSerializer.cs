using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public sealed class StringSerializer : SimpleClassTypeSerializer<String>
    {
        public StringSerializer(ClassRequirementType<String> type)
            : base(type)
        {
        }

        public override Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            String value,
            CancellationToken cancellationToken = default)
        {
            return intermediary.Writer.WriteValueAsync(value, cancellationToken);
        }
    }
}
