using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Persistables.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Drexel.Configurables.Persistables.Json
{
    public sealed class JsonRestorer : IRestorer, IDisposable
    {
        private readonly Stream stream;
        private readonly IReadOnlyCollection<IPersistableConfigurationRequirement> allowedSet;
        private readonly bool leaveOpen;

        public JsonRestorer(
            Stream stream,
            IReadOnlyCollection<IPersistableConfigurationRequirement> allowedSet,
            bool leaveOpen = true)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.allowedSet = allowedSet ?? throw new ArgumentNullException(nameof(allowedSet));
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Nothing to do, but we're reserving the interface for now.
        }

        public IPersistableConfiguration Restore(int millisecondsTimeout = -1)
        {
            Task<IPersistableConfiguration> task = this.RestoreAsync();
            task.Wait(millisecondsTimeout);
            return task.Result;
        }

        public async Task<IPersistableConfiguration> RestoreAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (StreamReader underlyingReader = new StreamReader(
                this.stream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: this.leaveOpen))
            using (JsonReader reader = new JsonTextReader(underlyingReader))
            {
                await reader.ReadStartObjectAsync(cancellationToken).ConfigureAwait(false);

                Version fileVersion = await JsonRestorer.ReadVersion(reader, cancellationToken).ConfigureAwait(false);
                if (!(fileVersion.Major == 1 && fileVersion.Minor == 0))
                {
                    throw new InvalidOperationException("Unsupported Version");
                }

                Guid configurationId =
                    await reader.ReadAsGuidAsync("Id", cancellationToken).ConfigureAwait(false);
                Version configurationVersion =
                    await reader.ReadAsVersionAsync("Version", cancellationToken).ConfigureAwait(false);

                IReadOnlyList<IPersistableConfigurationRequirement> keys = await JsonRestorer
                    .ReadKeys(reader, this.allowedSet, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private static async Task<Version> ReadVersion(JsonReader reader, CancellationToken cancellationToken)
        {
            await reader.ReadPropertyNameAsync("Version", cancellationToken).ConfigureAwait(false);
            return await reader.ReadAsVersionAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IReadOnlyList<IPersistableConfigurationRequirement>> ReadKeys(
            JsonReader reader,
            IReadOnlyCollection<IPersistableConfigurationRequirement> allowedSet,
            CancellationToken cancellationToken)
        {
            await reader.ReadStartArrayAsync("Keys", cancellationToken).ConfigureAwait(false);

            List<IPersistableConfigurationRequirement> keys = new List<IPersistableConfigurationRequirement>();

            KeyBuffer buffer = null;
            bool @continue = true;
            while (@continue)
            {
                await reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                switch (reader.TokenType)
                {
                    case JsonToken.EndArray:
                        if (buffer != null)
                        {
                            throw new JsonReaderException("Unexpected end of array");
                        }

                        @continue = false;
                        break;
                    case JsonToken.StartObject:
                        if (buffer != null)
                        {
                            throw new JsonReaderException("Unexpected start of object");
                        }

                        buffer = new KeyBuffer();
                        break;
                    case JsonToken.EndObject:
                        if (buffer == null)
                        {
                            throw new JsonReaderException("Premature end of object");
                        }

                        keys.Add(buffer.ToRequirement(allowedSet));
                        buffer = null;
                        break;
                    case JsonToken.PropertyName:
                        if (buffer == null)
                        {
                            throw new JsonReaderException("Unexpected property name");
                        }

                        string value = await reader.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        switch (value)
                        {
                            case null:
                                throw new JsonReaderException("Unexpected null");
                            case FieldNames.PropertyId:
                                buffer.SetId(value);
                                break;
                            case FieldNames.PropertyVersion:
                                buffer.SetVersion(value);
                                break;
                            case FieldNames.PropertyOfType:
                                throw new NotImplementedException("Not done");
                            case FieldNames.Value:
                                throw new NotImplementedException("Not done");
                            default:
                                throw new JsonReaderException("Unrecognized property name");
                        }

                        break;
                    default:
                        throw new JsonReaderException("Unexpected token type");
                }
            }

            return keys;
        }

        private class KeyBuffer
        {
            private Guid id;
            private Version version;
            private TypeBuffer type;
            private bool idInitialized;
            private bool versionInitialized;
            private bool typeInitialized;

            public KeyBuffer()
            {
                this.idInitialized = false;
                this.versionInitialized = false;
                this.typeInitialized = false;
            }

            public void SetId(string id)
            {
                if (this.idInitialized)
                {
                    throw new JsonReaderException("Duplicate field");
                }

                if (id == null)
                {
                    throw new JsonReaderException();
                }
                else if (Guid.TryParse(id, out Guid result))
                {
                    this.id = result;
                    this.idInitialized = true;
                }
                else
                {
                    throw new JsonReaderException();
                }
            }

            public void SetVersion(string version)
            {
                if (this.versionInitialized)
                {
                    throw new JsonReaderException("Duplicate field");
                }

                if (version == null)
                {
                    throw new JsonReaderException();
                }
                else if (Version.TryParse(version, out Version result))
                {
                    this.version = result;
                    this.versionInitialized = true;
                }
                else
                {
                    throw new JsonReaderException();
                }
            }

            public void SetOfType(TypeBuffer type)
            {
                if (this.typeInitialized)
                {
                    throw new JsonReaderException("Duplicate field");
                }

                this.type = type;
            }

            public IPersistableConfigurationRequirement ToRequirement(
                IReadOnlyCollection<IPersistableConfigurationRequirement> allowedSet)
            {
                if (this.idInitialized && this.versionInitialized && this.typeInitialized)
                {
                    return allowedSet.Single(
                        x =>
                        {
                            return x.Id == this.id
                                && x.Version == this.version
                                && x.OfType.Type == this.type.
                        });
                }
                else
                {
                    throw new JsonReaderException(); // End of object before all field were populated.
                }
            }
        }

        private class TypeBuffer
        {
            public 
        }
    }
}
