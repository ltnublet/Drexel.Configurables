using System.Globalization;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public sealed class BigIntegerSerializer : SimpleStructTypeSerializer<BigInteger>
    {
        public BigIntegerSerializer(StructRequirementType<BigInteger> type)
            : base(type)
        {
        }

        public override Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            BigInteger value,
            CancellationToken cancellationToken = default)
        {
            return intermediary.Writer.WriteValueAsync(
                value.ToString("R", CultureInfo.InvariantCulture),
                cancellationToken);
        }
    }
}
