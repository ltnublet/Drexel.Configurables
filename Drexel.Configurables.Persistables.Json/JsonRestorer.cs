using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Persistables.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Drexel.Configurables.Persistables.Json
{
    public sealed class JsonRestorer : IRestorer, IDisposable
    {
        private static readonly Version RestorerVersion = new Version(1, 0, 0, 0);

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
            if (!this.leaveOpen)
            {
                this.stream.Dispose();
            }
        }

        public IPersistableConfiguration Restore(int millisecondsTimeout = -1)
        {
            Task<IPersistableConfiguration> task = this.RestoreAsync();
            if (!task.Wait(millisecondsTimeout))
            {
                throw new TaskCanceledException();
            }

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
                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);

                await reader.ReadAsPropertyNameAsync(FieldNames.Version, cancellationToken).ConfigureAwait(false);
                Version version = await reader.ReadAsVersionAsync(cancellationToken).ConfigureAwait(false);
                if (version != JsonRestorer.RestorerVersion)
                {
                    throw new InvalidOperationException("Version not supported.");
                }

                IReadOnlyDictionary<Version, IReadOnlyDictionary<Guid, PersistableConfigurationRequirementType>> types =
                    await JsonRestorer.Types
                        .ReadAsTypes(
                            reader,
                            this.allowedSet.Select(x => x.OfType).Distinct().ToArray(),
                            cancellationToken)
                        .ConfigureAwait(false);
                IReadOnlyDictionary<Version, IReadOnlyDictionary<Guid, IPersistableConfigurationRequirement>> keys =
                    await JsonRestorer.Keys
                        .ReadAsKeys(
                            reader,
                            this.allowedSet,
                            types,
                            cancellationToken)
                        .ConfigureAwait(false);
                IReadOnlyDictionary<IPersistableConfigurationRequirement, object> mappings =
                    await JsonRestorer.Values
                        .ReadAsValues(
                            reader,
                            keys,
                            cancellationToken)
                        .ConfigureAwait(false);

                return new PersistableConfiguration(
                    new RequirementAdapter(keys.Values.SelectMany(x => x.Values)),
                    mappings,
                    null); // TODO: I'm still not convinced that IConfigurator should exist on the constructor...
            }
        }

        private static class Values
        {
            public static async Task<IReadOnlyDictionary<IPersistableConfigurationRequirement, object>> ReadAsValues(
                JsonReader reader,
                IReadOnlyDictionary<Version, IReadOnlyDictionary<Guid, IPersistableConfigurationRequirement>> keys,
                CancellationToken cancellationToken)
            {
                await reader.ReadAsPropertyNameAsync(FieldNames.Values, cancellationToken).ConfigureAwait(false);
                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);

                Dictionary<IPersistableConfigurationRequirement, object> retVal =
                    new Dictionary<IPersistableConfigurationRequirement, object>();

                bool @continue = true;
                while (@continue && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            Version version = Version.Parse((string)reader.Value);
                            await Values
                                .ReadAsVersionedValues(
                                    reader,
                                    keys[version],
                                    retVal,
                                    cancellationToken)
                                .ConfigureAwait(false);
                            break;
                        case JsonToken.EndObject:
                            @continue = false;
                            break;
                        default:
                            throw JsonRestorer.UnanticipatedTokenTypeException(reader.TokenType);
                    }
                }

                if (@continue)
                {
                    throw JsonRestorer.PrematureEndOfStreamException();
                }

                return retVal;
            }

            private static async Task ReadAsVersionedValues(
                JsonReader reader,
                IReadOnlyDictionary<Guid, IPersistableConfigurationRequirement> keys,
                Dictionary<IPersistableConfigurationRequirement, object> values,
                CancellationToken cancellationToken)
            {
                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);

                bool @continue = true;
                while (@continue && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            Guid id = Guid.Parse((string)reader.Value);
                            string buffer = await reader.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                            IPersistableConfigurationRequirement key = keys[id];
                            values.Add(key, key.OfType.Restore(buffer));
                            break;
                        case JsonToken.EndObject:
                            @continue = false;
                            break;
                        default:
                            throw JsonRestorer.UnanticipatedTokenTypeException(reader.TokenType);
                    }
                }

                if (@continue)
                {
                    throw JsonRestorer.PrematureEndOfStreamException();
                }
            }
        }

        private static class Keys
        {
            public static async Task<IReadOnlyDictionary<Version, IReadOnlyDictionary<Guid, IPersistableConfigurationRequirement>>> ReadAsKeys(
                JsonReader reader,
                IReadOnlyCollection<IPersistableConfigurationRequirement> allowedKeys,
                IReadOnlyDictionary<Version, IReadOnlyDictionary<Guid, PersistableConfigurationRequirementType>> types,
                CancellationToken cancellationToken)
            {
                await reader.ReadAsPropertyNameAsync(FieldNames.Keys, cancellationToken).ConfigureAwait(false);
                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);

                Dictionary<Version, IReadOnlyDictionary<Guid, IPersistableConfigurationRequirement>> retVal =
                    new Dictionary<Version, IReadOnlyDictionary<Guid, IPersistableConfigurationRequirement>>();

                bool @continue = true;
                while (@continue && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            Version version = Version.Parse((string)reader.Value);
                            retVal.Add(
                                version,
                                await JsonRestorer.Keys
                                    .ReadAsVersionedKeys(
                                        reader,
                                        allowedKeys,
                                        types,
                                        version,
                                        cancellationToken)
                                    .ConfigureAwait(false));
                            break;
                        case JsonToken.EndObject:
                            @continue = false;
                            break;
                        default:
                            throw JsonRestorer.UnanticipatedTokenTypeException(reader.TokenType);
                    }
                }

                if (@continue)
                {
                    throw JsonRestorer.PrematureEndOfStreamException();
                }

                return retVal;
            }

            private static async Task<IReadOnlyDictionary<Guid, IPersistableConfigurationRequirement>> ReadAsVersionedKeys(
                JsonReader reader,
                IReadOnlyCollection<IPersistableConfigurationRequirement> allowedKeys,
                IReadOnlyDictionary<Version, IReadOnlyDictionary<Guid, PersistableConfigurationRequirementType>> types,
                Version version,
                CancellationToken cancellationToken)
            {
                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);

                Dictionary<Guid, IPersistableConfigurationRequirement> retVal =
                    new Dictionary<Guid, IPersistableConfigurationRequirement>();

                bool @continue = true;
                while (@continue && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            Guid id = Guid.Parse((string)reader.Value);
                            retVal.Add(
                                id,
                                await JsonRestorer.Keys
                                    .ReadAsKey(
                                        reader,
                                        allowedKeys,
                                        types,
                                        version,
                                        id,
                                        cancellationToken)
                                    .ConfigureAwait(false));

                            break;
                        case JsonToken.EndObject:
                            @continue = false;
                            break;
                        default:
                            throw JsonRestorer.UnanticipatedTokenTypeException(reader.TokenType);
                    }
                }

                if (@continue)
                {
                    throw JsonRestorer.PrematureEndOfStreamException();
                }

                return retVal;
            }

            private static async Task<IPersistableConfigurationRequirement> ReadAsKey(
                JsonReader reader,
                IReadOnlyCollection<IPersistableConfigurationRequirement> allowedKeys,
                IReadOnlyDictionary<Version, IReadOnlyDictionary<Guid, PersistableConfigurationRequirementType>> types,
                Version version,
                Guid id,
                CancellationToken cancellationToken)
            {
                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);
                await reader.ReadAsPropertyNameAsync(FieldNames.OfType, cancellationToken).ConfigureAwait(false);

                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);
                await reader.ReadAsPropertyNameAsync(FieldNames.Id, cancellationToken).ConfigureAwait(false);
                Guid typeId = await reader.ReadAsGuidAsync(cancellationToken).ConfigureAwait(false);
                await reader.ReadAsPropertyNameAsync(FieldNames.Version, cancellationToken).ConfigureAwait(false);
                Version typeVersion = await reader.ReadAsVersionAsync(cancellationToken).ConfigureAwait(false);
                await reader.ReadAsEndObjectAsync(cancellationToken).ConfigureAwait(false);

                await reader.ReadAsEndObjectAsync(cancellationToken).ConfigureAwait(false);

                IPersistableConfigurationRequirement[] matchedRequiremeents = allowedKeys
                    .Where(
                        x =>
                        x.Id == id
                            && x.Version == version
                            && x.OfType == types[x.OfType.Version][x.OfType.Id])
                    .ToArray();

                if (matchedRequiremeents.Length == 0)
                {
                    throw JsonRestorer.UnrecognizedConfigurationRequirement(
                        version,
                        id,
                        typeVersion,
                        typeId);
                }
                else if (matchedRequiremeents.Length > 1)
                {
                    throw JsonRestorer.AmbiguousConfigurationRequirement(
                        version,
                        id,
                        typeVersion,
                        typeId);
                }
                else
                {
                    return matchedRequiremeents[0];
                }
            }
        }

        private static class Types
        {
            public static async Task<IReadOnlyDictionary<Version, IReadOnlyDictionary<Guid, PersistableConfigurationRequirementType>>> ReadAsTypes(
               JsonReader reader,
               IReadOnlyCollection<PersistableConfigurationRequirementType> allowedTypes,
               CancellationToken cancellationToken)
            {
                await reader.ReadAsPropertyNameAsync(FieldNames.Types, cancellationToken).ConfigureAwait(false);
                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);

                Dictionary<Version, IReadOnlyDictionary<Guid, PersistableConfigurationRequirementType>> retVal =
                    new Dictionary<Version, IReadOnlyDictionary<Guid, PersistableConfigurationRequirementType>>();

                bool @continue = true;
                while (@continue && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            Version version = Version.Parse((string)reader.Value);
                            retVal.Add(
                                version,
                                await JsonRestorer.Types
                                    .ReadAsVersionedTypes(
                                        reader,
                                        allowedTypes,
                                        version,
                                        cancellationToken)
                                    .ConfigureAwait(false));
                            break;
                        case JsonToken.EndObject:
                            @continue = false;
                            break;
                        default:
                            throw JsonRestorer.UnanticipatedTokenTypeException(reader.TokenType);
                    }
                }

                if (@continue)
                {
                    throw JsonRestorer.PrematureEndOfStreamException();
                }

                return retVal;
            }

            private static async Task<IReadOnlyDictionary<Guid, PersistableConfigurationRequirementType>> ReadAsVersionedTypes(
                JsonReader reader,
                IReadOnlyCollection<PersistableConfigurationRequirementType> allowedTypes,
                Version version,
                CancellationToken cancellationToken)
            {
                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);

                Dictionary<Guid, PersistableConfigurationRequirementType> retVal =
                    new Dictionary<Guid, PersistableConfigurationRequirementType>();

                bool @continue = true;
                while (@continue && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            Guid id = Guid.Parse((string)reader.Value);
                            retVal.Add(
                                id,
                                await JsonRestorer.Types
                                    .ReadAsType(
                                        reader,
                                        allowedTypes,
                                        version,
                                        id,
                                        cancellationToken)
                                    .ConfigureAwait(false));

                            break;
                        case JsonToken.EndObject:
                            @continue = false;
                            break;
                        default:
                            throw JsonRestorer.UnanticipatedTokenTypeException(reader.TokenType);
                    }
                }

                if (@continue)
                {
                    throw JsonRestorer.PrematureEndOfStreamException();
                }

                return retVal;
            }

            private static async Task<PersistableConfigurationRequirementType> ReadAsType(
                JsonReader reader,
                IReadOnlyCollection<PersistableConfigurationRequirementType> allowedTypes,
                Version version,
                Guid id,
                CancellationToken cancellationToken)
            {
                await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);
                await reader.ReadAsPropertyNameAsync(FieldNames.Type, cancellationToken).ConfigureAwait(false);
                Type type = await reader.ReadAsTypeAsync(cancellationToken).ConfigureAwait(false);
                await reader.ReadAsEndObjectAsync(cancellationToken).ConfigureAwait(false);

                PersistableConfigurationRequirementType[] matchedTypes = allowedTypes
                    .Where(x => x.Id == id && x.Version == version && x.Type == type)
                    .ToArray();

                if (matchedTypes.Length == 0)
                {
                    throw JsonRestorer.UnrecognizedConfigurationRequirementType(version, id, type);
                }
                else if (matchedTypes.Length > 1)
                {
                    throw JsonRestorer.AmbiguousConfigurationRequirementType(version, id, type);
                }
                else
                {
                    return matchedTypes[0];
                }
            }
        }

        [System.Diagnostics.DebuggerHidden]
        private static Exception PrematureEndOfStreamException()
        {
            return new InvalidOperationException("Premature end of stream.");
        }

        [System.Diagnostics.DebuggerHidden]
        private static Exception UnanticipatedTokenTypeException(JsonToken token)
        {
            return new InvalidOperationException("Unanticipated token type.");
        }

        [System.Diagnostics.DebuggerHidden]
        private static Exception UnrecognizedConfigurationRequirementType(Version version, Guid id, Type type)
        {
            return new InvalidOperationException("Unrecognized configuration requirement type.");
        }

        private static Exception AmbiguousConfigurationRequirementType(Version version, Guid id, Type type)
        {
            return new InvalidOperationException("Ambiguous configuration requirement type.");
        }

        [System.Diagnostics.DebuggerHidden]
        private static Exception UnrecognizedConfigurationRequirement(
            Version version,
            Guid id,
            Version typeVersion,
            Guid typeId)
        {
            return new InvalidOperationException("Unrecognized configuration requirement.");
        }

        private static Exception AmbiguousConfigurationRequirement(
            Version version,
            Guid id,
            Version typeVersion,
            Guid typeId)
        {
            return new InvalidOperationException("Ambiguous configuration requirement.");
        }

        private class RequirementAdapter : IPersistableConfigurationRequirementSource
        {
            public RequirementAdapter(IEnumerable<IPersistableConfigurationRequirement> requirements)
            {
                this.Requirements = requirements.ToList();
            }

            public IReadOnlyList<IPersistableConfigurationRequirement> Requirements { get; }

            IReadOnlyList<IConfigurationRequirement> IRequirementSource.Requirements => this.Requirements;
        }
    }
}
