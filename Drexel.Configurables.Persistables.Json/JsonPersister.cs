using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Persistables.Contracts;
using Newtonsoft.Json;

namespace Drexel.Configurables.Persistables.Json
{
    public class JsonPersister : IPersister
    {
        private const int BufferSize = 1024;
        private readonly Stream stream;

        public JsonPersister(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public async Task PersistAsync(
            IPersistableConfiguration configuration,
            CancellationToken token)
        {
            using (StreamWriter underlyingWriter = new StreamWriter(
                this.stream,
                Encoding.UTF8,
                bufferSize: JsonPersister.BufferSize,
                leaveOpen: true))
            using (JsonTextWriter writer = new JsonTextWriter(underlyingWriter))
            {
                await writer.WriteStartObjectAsync(token).ConfigureAwait(false);

                await writer.WritePropertyNameAsync("Keys", token).ConfigureAwait(false);
                await writer.WriteStartArrayAsync(token).ConfigureAwait(false);

                foreach (IPersistableConfigurationRequirement requirement in configuration.Keys)
                {
                    await writer.WriteStartObjectAsync(token).ConfigureAwait(false);

                    await writer.WritePropertyNameAsync("Id", token).ConfigureAwait(false);
                    await writer.WriteValueAsync(requirement.Id.ToString(), token).ConfigureAwait(false);

                    await writer.WritePropertyNameAsync("Version", token).ConfigureAwait(false);
                    await writer.WriteValueAsync(requirement.Version.ToString(), token).ConfigureAwait(false);

                    await writer.WritePropertyNameAsync("OfType", token).ConfigureAwait(false);
                    await writer.WriteStartObjectAsync(token).ConfigureAwait(false);
                    await writer.WritePropertyNameAsync("Version", token).ConfigureAwait(false);
                    await writer.WriteValueAsync(requirement.OfType.Version.ToString(), token).ConfigureAwait(false);
                    await writer.WritePropertyNameAsync("Type", token).ConfigureAwait(false);
                    await writer.WriteValueAsync(requirement.OfType.Type.ToString(), token).ConfigureAwait(false);
                    await writer.WriteEndObjectAsync(token).ConfigureAwait(false);

                    await writer.WritePropertyNameAsync("Value", token).ConfigureAwait(false);
                    await writer
                        .WriteValueAsync(
                            requirement.OfType.Encode(configuration[requirement]),
                            token)
                        .ConfigureAwait(false);

                    await writer.WriteEndObjectAsync(token).ConfigureAwait(false);
                }

                await writer.WriteEndArrayAsync(token).ConfigureAwait(false);
                await writer.WriteEndObjectAsync(token).ConfigureAwait(false);
            }
        }
    }
}
