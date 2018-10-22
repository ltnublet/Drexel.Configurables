using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Persistables.Contracts;
using Newtonsoft.Json;

namespace Drexel.Configurables.Persistables.Json
{
    /// <summary>
    /// Persists an <see cref="IPersistableConfiguration"/> for later retrieval using JSON formatting.
    /// </summary>
    public sealed class JsonPersister : IPersister, IDisposable
    {
        private const int BufferSize = 1024;
        private static readonly string PersisterVersion = new Version(1, 0, 0, 0).ToString();

        private readonly Stream stream;
        private readonly bool pretty;
        private readonly bool leaveOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPersister"/> class.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to perform write operations upon.
        /// </param>
        /// <param name="pretty">
        /// When <see langword="true"/>, persist operations will produce pretty-printed JSON.
        /// </param>
        /// <param name="leaveOpen">
        /// When <see langword="true"/>, the <paramref name="stream"/> will not be closed upon performing a persist
        /// operation.
        /// </param>
        public JsonPersister(
            Stream stream,
            bool pretty = true,
            bool leaveOpen = true)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.pretty = pretty;
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Nothing to do, but we're reserving the interface for now.
        }

        /// <summary>
        /// Persists the specified <see cref="IPersistableConfiguration"/> <paramref name="configuration"/> for later
        /// restoration.
        /// </summary>
        /// <param name="configuration">
        /// The configuration to persist.
        /// </param>
        /// <param name="millisecondsTimeout">
        /// If persisting doesn't complete within this number of millisecond, it will be aborted. To never abort,
        /// supply a value of <code>-1</code>.
        /// </param>
        public void Persist(IPersistableConfiguration configuration, int millisecondsTimeout = -1) =>
            this.PersistAsync(configuration, CancellationToken.None).Wait(millisecondsTimeout);

        /// <summary>
        /// Asynchronously persists the specified <see cref="IPersistableConfiguration"/>
        /// <paramref name="configuration"/> for later restoration.
        /// </summary>
        /// <param name="configuration">
        /// The configuration to persist.
        /// </param>
        /// <param name="token">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the execution state of the persist operation.
        /// </returns>
        public async Task PersistAsync(
            IPersistableConfiguration configuration,
            CancellationToken token = default(CancellationToken))
        {
            using (StreamWriter underlyingWriter = new StreamWriter(
                this.stream,
                Encoding.UTF8,
                bufferSize: JsonPersister.BufferSize,
                leaveOpen: this.leaveOpen))
            using (JsonTextWriter writer =
                new JsonTextWriter(underlyingWriter)
                {
                    Formatting = this.pretty ? Formatting.Indented : Formatting.None
                })
            {
                await writer.WriteStartObjectAsync(token).ConfigureAwait(false);

                await writer.WritePropertyNameAsync(FieldNames.PersisterVersion, token).ConfigureAwait(false);
                await writer.WriteValueAsync(JsonPersister.PersisterVersion, token).ConfigureAwait(false);

                await writer.WritePropertyNameAsync(FieldNames.Keys, token).ConfigureAwait(false);
                await writer.WriteStartArrayAsync(token).ConfigureAwait(false);

                foreach (IPersistableConfigurationRequirement requirement in configuration.Keys)
                {
                    await writer.WriteStartObjectAsync(token).ConfigureAwait(false);

                    await writer.WritePropertyNameAsync(FieldNames.PropertyId, token).ConfigureAwait(false);
                    await writer.WriteValueAsync(requirement.Id.ToString(), token).ConfigureAwait(false);

                    await writer.WritePropertyNameAsync(FieldNames.PropertyVersion, token).ConfigureAwait(false);
                    await writer.WriteValueAsync(requirement.Version.ToString(), token).ConfigureAwait(false);

                    await writer.WritePropertyNameAsync(FieldNames.PropertyOfType, token).ConfigureAwait(false);
                    await writer.WriteStartObjectAsync(token).ConfigureAwait(false);
                    await writer.WritePropertyNameAsync(FieldNames.PropertyOfTypeVersion, token).ConfigureAwait(false);
                    await writer.WriteValueAsync(requirement.OfType.Version.ToString(), token).ConfigureAwait(false);
                    await writer.WritePropertyNameAsync(FieldNames.PropertyOfTypeType, token).ConfigureAwait(false);
                    await writer.WriteValueAsync(requirement.OfType.Type.ToString(), token).ConfigureAwait(false);
                    await writer.WriteEndObjectAsync(token).ConfigureAwait(false);

                    await writer.WritePropertyNameAsync(FieldNames.Value, token).ConfigureAwait(false);
                    await writer
                        .WriteValueAsync(
                            requirement.OfType.Encode(configuration[requirement]),
                            token)
                        .ConfigureAwait(false);

                    await writer.WriteEndObjectAsync(token).ConfigureAwait(false);
                }

                await writer.WriteEndArrayAsync(token).ConfigureAwait(false);
                await writer.WriteEndObjectAsync(token).ConfigureAwait(false);

                await writer.FlushAsync(token).ConfigureAwait(false);
            }
        }
    }
}
