using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Drexel.Configurables.Persistables.Json
{
    internal static class JsonWriterExtensions
    {
        public static Task WritePropertyNameAsync(
            this JsonWriter writer,
            Version version,
            CancellationToken cancellationToken) => writer.WritePropertyNameAsync(
                version.ToString(),
                cancellationToken);

        public static Task WritePropertyNameAsync(
            this JsonWriter writer,
            Guid guid,
            CancellationToken cancellationToken) => writer.WritePropertyNameAsync(
                guid.ToString(),
                cancellationToken);

        public static Task WriteValueAsync(
            this JsonWriter writer,
            Version version,
            CancellationToken cancellationToken) => writer.WriteValueAsync(
                version.ToString(),
                cancellationToken);
    }
}
