using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Drexel.Configurables.Persistables.Json
{
    internal static class JsonReaderExtensions
    {
        public static async Task<Guid> ReadAsGuidAsync(
            this JsonReader reader,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string value = await reader.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonReaderException();
            }
            else if (Guid.TryParse(value, out Guid result))
            {
                return result;
            }
            else
            {
                throw new JsonReaderException();
            }
        }

        public static async Task<Guid> ReadAsGuidAsync(
            this JsonReader reader,
            string fieldName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadPropertyNameAsync(fieldName, cancellationToken).ConfigureAwait(false);
            return await reader.ReadAsGuidAsync(cancellationToken).ConfigureAwait(false);
        }

        public static async Task ReadPropertyNameAsync(
            this JsonReader reader,
            string fieldName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string value = await reader.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw new JsonReaderException();
            }

            if (value != fieldName)
            {
                throw new JsonReaderException();
            }
        }

        public static async Task ReadStartArrayAsync(
            this JsonReader reader,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonReaderException();
            }
        }

        public static async Task ReadStartArrayAsync(
            this JsonReader reader,
            string fieldName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadPropertyNameAsync(fieldName, cancellationToken).ConfigureAwait(false);
            await reader.ReadStartArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        public static async Task ReadStartObjectAsync(
            this JsonReader reader,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonReaderException();
            }
        }

        public static async Task ReadStartObjectAsync(
            this JsonReader reader,
            string fieldName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadPropertyNameAsync(fieldName, cancellationToken).ConfigureAwait(false);
            await reader.ReadStartObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Version> ReadAsVersionAsync(
            this JsonReader reader,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string value = await reader.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonReaderException();
            }
            else if (Version.TryParse(value, out Version result))
            {
                return result;
            }
            else
            {
                throw new JsonReaderException();
            }
        }
        public static async Task<Version> ReadAsVersionAsync(
            this JsonReader reader,
            string fieldName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadPropertyNameAsync(fieldName, cancellationToken).ConfigureAwait(false);
            return await reader.ReadAsVersionAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
