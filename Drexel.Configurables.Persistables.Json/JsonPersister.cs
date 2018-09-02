using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.External;
using Drexel.Configurables.Persistables.Contracts;

namespace Drexel.Configurables.Persistables.Json
{
    public class JsonPersister : IPersister
    {
        private readonly Stream stream;

        public JsonPersister(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public async Task Persist(
            IPersistableConfiguration configuration,
            CancellationToken token)
        {
            JsonWriter writer = new JsonWriter(this.stream, pretty: true);

            try
            {
                await writer.WriteObjectStart(token).ConfigureAwait(false);

                await writer.WritePropertyName("Keys", token).ConfigureAwait(false);
                await writer.WriteArrayStart(token).ConfigureAwait(false);

                foreach (IPersistableConfigurationRequirement requirement in configuration.Keys)
                {
                    await writer.WriteObjectStart(token).ConfigureAwait(false);

                    await writer.WritePropertyName("Id", token).ConfigureAwait(false);
                    await writer.WriteValue(requirement.Id.ToString(), token).ConfigureAwait(false);

                    await writer.WritePropertyName("Version", token).ConfigureAwait(false);
                    await writer.WriteValue(requirement.Version.ToString(), token).ConfigureAwait(false);

                    await writer.WritePropertyName("OfType", token).ConfigureAwait(false);
                    await writer.WriteObjectStart(token).ConfigureAwait(false);
                    await writer.WritePropertyName("Version", token).ConfigureAwait(false);
                    await writer.WriteValue(requirement.OfType.Version.ToString(), token).ConfigureAwait(false);
                    await writer.WritePropertyName("Type", token).ConfigureAwait(false);
                    await writer.WriteValue(requirement.OfType.Type.ToString(), token).ConfigureAwait(false);
                    await writer.WriteObjectEnd(token).ConfigureAwait(false);

                    await writer.WritePropertyName("Value", token).ConfigureAwait(false);
                    await writer
                        .WriteValue(
                            requirement.OfType.Encode(configuration[requirement]),
                            token)
                        .ConfigureAwait(false);

                    await writer.WriteObjectEnd(token).ConfigureAwait(false);
                }

                await writer.WriteArrayEnd(token).ConfigureAwait(false);
                await writer.WriteObjectEnd(token).ConfigureAwait(false);
            }
            finally
            {
                writer.Dispose();
            }
        }
    }
}
