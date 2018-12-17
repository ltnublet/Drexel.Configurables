using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Json.Exceptions;
using Newtonsoft.Json;

namespace Drexel.Configurables.Json.V1
{
    public class JsonConfigurationRestorer : IConfigurationRestorer, IDisposable
    {
        private readonly JsonStreamReader reader;
        private bool isDisposed;

        public JsonConfigurationRestorer(
            Stream stream,
            bool leaveOpen,
            int bufferSize = 1024)
        {
            this.reader = new JsonStreamReader(stream, leaveOpen, bufferSize);
            this.isDisposed = false;
        }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
                this.reader.Dispose();
            }
        }

        public async Task<IConfiguration> RestoreAsync(
            IReadOnlyCollection<IRequirement> requirementSet,
            Func<IReadOnlyCollection<IRequirement>, IReadOnlyDictionary<IRequirement, object>> unpersistableRequirementsCallback,
            CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();

            this.reader.ReadAsStartObject();

            Version fileVersion = this.reader.ReadAsVersion(FieldNames.Version);
            if (fileVersion != Shared.Version)
            {
                throw new VersionNotSupportedException(fileVersion);
            }

            // Read `types` array.
            Dictionary<Guid, Version> types = new Dictionary<Guid, Version>();
            this.reader.ReadAsPropertyName(FieldNames.Types);
            this.reader.ReadAsArray(
                new Dictionary<JsonToken, Func<JsonStreamReader, bool>>()
                {
                    [JsonToken.StartObject] =
                        (x) =>
                        {
                            Guid typeId = x.ReadAsGuid(FieldNames.TypeFieldNames.Id);
                            Version typeVersion = x.ReadAsVersion(FieldNames.TypeFieldNames.Version);

                            x.ReadAsEndObject();

                            types.Add(typeId, typeVersion);

                            return false;
                        }
                });

            // Read `keys` array.
            Dictionary<Guid, Guid> keys = new Dictionary<Guid, Guid>();
            this.reader.ReadAsPropertyName(FieldNames.Keys);
            this.reader.ReadAsArray(
                new Dictionary<JsonToken, Func<JsonStreamReader, bool>>()
                {
                    [JsonToken.StartObject] =
                        (x) =>
                        {
                            Guid id = x.ReadAsGuid(FieldNames.KeyFieldNames.Id);
                            Guid typeId = x.ReadAsGuid(FieldNames.KeyFieldNames.TypeId);

                            x.ReadAsEndObject();

                            keys.Add(id, typeId);

                            return false;
                        }
                });

            // Foreach each requirement,
            //  foreach requirement it depends on,
            //   if it has no dependencies, get it
            //   if it has dependencies, recursively call ourself

            Dictionary<IRequirement, object> alreadyRestored = new Dictionary<IRequirement, object>();
            void RestoreRequirement(IRequirement toRestore)
            {
                if (alreadyRestored.ContainsKey(toRestore))
                {
                    return;
                }
                else
                {
                    foreach (IRequirement requirement in toRestore.DependsOn)
                    {

                    }
                }
            }

            foreach (IRequirement requirement in requirementSet)
            {
                requirement.Type.
            }
        }

        [System.Diagnostics.DebuggerHidden]
        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(nameof(JsonConfigurationRestorer));
            }
        }
    }
}
