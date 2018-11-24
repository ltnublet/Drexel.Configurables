using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
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
        private const string PersisterVersion = "1.0.0.0";

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
            if (!this.leaveOpen)
            {
                this.stream.Dispose();
            }
        }

        /// <summary>
        /// Persists the specified <see cref="IPersistableConfiguration"/> <paramref name="configuration"/> for later
        /// restoration.
        /// </summary>
        /// <param name="configuration">
        /// The configuration to persist.
        /// </param>
        /// <param name="millisecondsTimeout">
        /// If persisting doesn't complete within this number of milliseconds, it will be aborted. To never abort,
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
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the execution state of the persist operation.
        /// </returns>
        public async Task PersistAsync(
            IPersistableConfiguration configuration,
            CancellationToken cancellationToken = default(CancellationToken))
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
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

                await writer.WritePropertyNameAsync(FieldNames.Version, cancellationToken).ConfigureAwait(false);
                await writer.WriteValueAsync(JsonPersister.PersisterVersion, cancellationToken).ConfigureAwait(false);

                await JsonPersister.Types.WriteTypes(writer, configuration, cancellationToken).ConfigureAwait(false);
                await JsonPersister.Keys.WriteKeys(writer, configuration, cancellationToken).ConfigureAwait(false);
                await JsonPersister.Values.WriteValues(writer, configuration, cancellationToken).ConfigureAwait(false);

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static class Values
        {
            public static async Task WriteValues(
                JsonWriter writer,
                IPersistableConfiguration configuration,
                CancellationToken cancellationToken)
            {
                await writer.WritePropertyNameAsync(FieldNames.Values, cancellationToken).ConfigureAwait(false);
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

                IEnumerable<IMapping<IPersistableConfigurationRequirement>> asEnumerable =
                    configuration as IEnumerable<IMapping<IPersistableConfigurationRequirement>>;
                foreach (IGrouping<Version, IMapping<IPersistableConfigurationRequirement>> mappingsByVersion in
                    asEnumerable.GroupBy(x => x.Key.Version))
                {
                    await Values
                        .WriteMappings(
                            writer,
                            mappingsByVersion.Key,
                            mappingsByVersion,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }

            private static async Task WriteMappings(
                JsonWriter writer,
                Version version,
                IEnumerable<IMapping<IPersistableConfigurationRequirement>> mappings,
                CancellationToken cancellationToken)
            {
                await writer.WritePropertyNameAsync(version, cancellationToken).ConfigureAwait(false);
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

                foreach (IMapping<IPersistableConfigurationRequirement> mapping in mappings)
                {
                    await Values.WriteMapping(writer, mapping, cancellationToken).ConfigureAwait(false);
                }

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }

            private static async Task WriteMapping(
                JsonWriter writer,
                IMapping<IPersistableConfigurationRequirement> mapping,
                CancellationToken cancellationToken)
            {
                await writer.WritePropertyNameAsync(mapping.Key.Id, cancellationToken).ConfigureAwait(false);
                await writer.WriteValueAsync(mapping.Value, cancellationToken).ConfigureAwait(false);
            }
        }

        private static class Keys
        {
            public static async Task WriteKeys(
                JsonTextWriter writer,
                IPersistableConfiguration configuration,
                CancellationToken cancellationToken)
            {
                await writer.WritePropertyNameAsync(FieldNames.Keys, cancellationToken).ConfigureAwait(false);
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
                foreach (IGrouping<Version, IPersistableConfigurationRequirement> requirementsByVersion in
                    configuration.Keys.GroupBy(x => x.Version))
                {
                    await JsonPersister.Keys
                        .WriteConfigurationRequirementGroup(
                            writer,
                            requirementsByVersion.Key,
                            requirementsByVersion,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }

            private static async Task WriteConfigurationRequirementGroup(
                JsonTextWriter writer,
                Version version,
                IEnumerable<IPersistableConfigurationRequirement> requirements,
                CancellationToken cancellationToken)
            {
                await writer.WritePropertyNameAsync(version, cancellationToken).ConfigureAwait(false);
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
                foreach (IPersistableConfigurationRequirement requirement in requirements)
                {
                    await JsonPersister.Keys
                        .WriteConfigurationRequirement(writer, requirement, cancellationToken)
                        .ConfigureAwait(false);
                }

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }

            private static async Task WriteConfigurationRequirement(
                JsonTextWriter writer,
                IPersistableConfigurationRequirement requirement,
                CancellationToken cancellationToken)
            {
                await writer.WritePropertyNameAsync(requirement.Id, cancellationToken).ConfigureAwait(false);
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

                await JsonPersister.Keys
                    .WriteConfigureationRequirementType(writer, requirement.OfType, cancellationToken)
                    .ConfigureAwait(false);

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }

            private static async Task WriteConfigureationRequirementType(
                JsonTextWriter writer,
                PersistableConfigurationRequirementType requirementType,
                CancellationToken cancellationToken)
            {
                await writer.WritePropertyNameAsync(FieldNames.OfType, cancellationToken).ConfigureAwait(false);
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

                await writer.WritePropertyNameAsync(FieldNames.Id, cancellationToken).ConfigureAwait(false);
                await writer.WriteValueAsync(requirementType.Id, cancellationToken).ConfigureAwait(false);
                await writer.WritePropertyNameAsync(FieldNames.Version, cancellationToken).ConfigureAwait(false);
                await JsonWriterExtensions
                    .WriteValueAsync(writer, requirementType.Version, cancellationToken)
                    .ConfigureAwait(false);

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static class Types
        {
            public static async Task WriteTypes(
                JsonTextWriter writer,
                IPersistableConfiguration configuration,
                CancellationToken cancellationToken)
            {
                await writer.WritePropertyNameAsync(FieldNames.Types, cancellationToken).ConfigureAwait(false);
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
                foreach (IGrouping<Version, PersistableConfigurationRequirementType> typesByVersion in configuration
                    .Keys
                    .Select(x => x.OfType)
                    .Distinct()
                    .GroupBy(x => x.Version))
                {
                    await JsonPersister.Types
                        .WriteConfigurationRequirementTypeGroup(
                            writer,
                            typesByVersion.Key,
                            typesByVersion,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }

            private static async Task WriteConfigurationRequirementTypeGroup(
                JsonWriter writer,
                Version version,
                IEnumerable<PersistableConfigurationRequirementType> types,
                CancellationToken cancellationToken)
            {
                await writer.WritePropertyNameAsync(version, cancellationToken).ConfigureAwait(false);
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
                foreach (PersistableConfigurationRequirementType type in types)
                {
                    await writer.WritePropertyNameAsync(type.Id, cancellationToken).ConfigureAwait(false);
                    await JsonPersister.Types
                        .WriteConfigurationRequirementType(writer, type, cancellationToken)
                        .ConfigureAwait(false);
                }

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }

            private static async Task WriteConfigurationRequirementType(
                JsonWriter writer,
                PersistableConfigurationRequirementType type,
                CancellationToken cancellationToken)
            {
                await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

                await writer.WritePropertyNameAsync(FieldNames.Type, cancellationToken).ConfigureAwait(false);
                await writer.WriteValueAsync(type.Type.AssemblyQualifiedName, cancellationToken).ConfigureAwait(false);

                await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
