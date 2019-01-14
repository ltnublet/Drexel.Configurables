using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft
{
    internal static class JsonTextWriterExtensionMethods
    {
        public static async Task WriteValueAsync(
            this JsonTextWriter writer,
            Version version,
            CancellationToken cancellationToken = default)
        {
            await writer.WriteValueAsync(version.ToString(), cancellationToken).ConfigureAwait(false);
        }
    }
}
