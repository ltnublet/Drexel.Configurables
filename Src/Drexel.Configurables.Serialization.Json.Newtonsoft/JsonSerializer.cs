using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Newtonsoft.Json;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft
{
    public class JsonSerializer : ISerializer<NewtonsoftIntermediary>, IDisposable
    {
        private readonly JsonTextWriter writer;
        private readonly NewtonsoftIntermediary intermediary;

        private bool disposed;

        public JsonSerializer(
            TypeSerializerDictionary<NewtonsoftIntermediary> typeSerializers,
            Stream underlyingStream,
            bool pretty = true,
            int bufferSize = 1024,
            bool leaveOpen = false)
        {
            this.Supported = typeSerializers ?? throw new ArgumentNullException(nameof(typeSerializers));

            if (underlyingStream == null)
            {
                throw new ArgumentNullException(nameof(underlyingStream));
            }

            this.disposed = false;
            this.writer = new JsonTextWriter(
                new StreamWriter(
                    underlyingStream,
                    Encoding.UTF8,
                    bufferSize,
                    leaveOpen));
            this.writer.Formatting = pretty ? Formatting.Indented : Formatting.None;
            this.intermediary = new NewtonsoftIntermediary(this.writer);
        }

        public TypeSerializerDictionary<NewtonsoftIntermediary> Supported { get; }

        public async Task SerializeAsync(
            Configuration configuration,
            CancellationToken cancellationToken = default)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            await this.writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.Version, cancellationToken).ConfigureAwait(false);
            await JsonTextWriterExtensionMethods.WriteValueAsync(this.writer, new Version(1, 0, 0, 0), cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.Configuration, cancellationToken).ConfigureAwait(false);
            await this.WriteConfigurationAsync(configuration, cancellationToken).ConfigureAwait(false);

            await this.writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteConfigurationAsync(
            Configuration configuration,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.Types, cancellationToken).ConfigureAwait(false);
            await this.WriteRequirementTypesAsync(configuration.Requirements.Select(x => x.Value.Type), cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.Requirements, cancellationToken).ConfigureAwait(false);
            await this.WriteRequirementsAsync(configuration.Requirements.Values, cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.Values, cancellationToken).ConfigureAwait(false);
            await this.WriteValuesAsync(configuration, cancellationToken).ConfigureAwait(false);

            await this.writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteRequirementTypesAsync(
            IEnumerable<RequirementType> types,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.writer.WriteStartArrayAsync(cancellationToken).ConfigureAwait(false);

            foreach (RequirementType type in types)
            {
                await this.WriteRequirementTypeAsync(type, cancellationToken).ConfigureAwait(false);
            }

            await this.writer.WriteEndArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteRequirementTypeAsync(
            RequirementType type,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.RequirementTypeNames.Id, cancellationToken).ConfigureAwait(false);
            await this.writer.WriteValueAsync(type.Id, cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.RequirementTypeNames.Type, cancellationToken).ConfigureAwait(false);
            await this.writer.WriteValueAsync(type.Type.AssemblyQualifiedName, cancellationToken).ConfigureAwait(false);

            await this.writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteRequirementsAsync(
            IEnumerable<Requirement> requirements,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.writer.WriteStartArrayAsync(cancellationToken).ConfigureAwait(false);

            foreach (Requirement requirement in requirements)
            {
                await this.WriteRequirementAsync(requirement, cancellationToken).ConfigureAwait(false);
            }

            await this.writer.WriteEndArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteRequirementAsync(
            Requirement requirement,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.RequirementNames.Id, cancellationToken).ConfigureAwait(false);
            await this.writer.WriteValueAsync(requirement.Id, cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.RequirementNames.Type, cancellationToken).ConfigureAwait(false);
            await this.writer.WriteValueAsync(requirement.Type.Id, cancellationToken).ConfigureAwait(false);

            await this.writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteValuesAsync(
            Configuration values,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.writer.WriteStartArrayAsync(cancellationToken).ConfigureAwait(false);

            foreach (Requirement requirement in values.Requirements.Values)
            {
                if (values.TryGetValue(requirement, out object? valueBuffer))
                {
                    await this.WriteValueAsync(requirement, valueBuffer, cancellationToken).ConfigureAwait(false);
                }
                else if (values.TryGetValues(requirement, out IEnumerable? collectionBuffer))
                {
                    await this.WriteValueAsync(requirement, collectionBuffer, cancellationToken).ConfigureAwait(false);
                }
            }

            await this.writer.WriteEndArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteValueAsync(
            Requirement requirement,
            object? value,
            CancellationToken cancellationToken)
        {
            await this.WriteValueHeader(requirement, cancellationToken).ConfigureAwait(false);
            await this.Supported[requirement.Type].SerializeAsync(this.intermediary, value, cancellationToken).ConfigureAwait(false);
            await this.WriteValueFooter(requirement, cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteValuesAsync(
            Requirement requirement,
            IEnumerable? values,
            CancellationToken cancellationToken)
        {
            await this.WriteValueHeader(requirement, cancellationToken);
            await this.Supported[requirement.Type].SerializeAsync(this.intermediary, values, cancellationToken).ConfigureAwait(false);
            await this.WriteValueFooter(requirement, cancellationToken);
        }

        private async Task WriteValueHeader(
            Requirement requirement,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.ValueNames.Key, cancellationToken).ConfigureAwait(false);
            await this.writer.WriteValueAsync(requirement.Id, cancellationToken).ConfigureAwait(false);

            await this.writer.WritePropertyNameAsync(FieldNames.ValueNames.Value, cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteValueFooter(
            Requirement requirement,
            CancellationToken cancellationToken)
        {
            await this.writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        ~JsonSerializer()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (!this.disposed)
            {
                this.disposed = true;
                ((IDisposable)this.writer).Dispose();
            }
        }
    }
}
