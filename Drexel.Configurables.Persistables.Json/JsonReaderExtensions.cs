using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Drexel.Configurables.Persistables.Json
{
    internal static class JsonReaderExtensions
    {
        public static async Task<Type> ReadAsTypeAsync(
            this JsonReader reader,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string value = await reader.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonReaderException();
            }

            return Type.GetType(value, true);
        }

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
            await reader.ReadAsPropertyNameAsync(fieldName, cancellationToken).ConfigureAwait(false);
            return await reader.ReadAsGuidAsync(cancellationToken).ConfigureAwait(false);
        }

        public static async Task ReadAsPropertyNameAsync(
            this JsonReader reader,
            string fieldName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw new JsonReaderException();
            }

            if ((string)reader.Value != fieldName)
            {
                throw new JsonReaderException();
            }
        }

        public static async Task ReadAsStartArrayAsync(
            this JsonReader reader,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonReaderException();
            }
        }

        public static async Task ReadAsStartArrayAsync(
            this JsonReader reader,
            string fieldName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadAsPropertyNameAsync(fieldName, cancellationToken).ConfigureAwait(false);
            await reader.ReadAsStartArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        public static async Task ReadAsStartObjectAsync(
            this JsonReader reader,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonReaderException();
            }
        }

        public static async Task ReadAsEndObjectAsync(
            this JsonReader reader,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            if (reader.TokenType != JsonToken.EndObject)
            {
                throw new JsonReaderException();
            }
        }

        public static async Task ReadAsStartObjectAsync(
            this JsonReader reader,
            string fieldName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.ReadAsPropertyNameAsync(fieldName, cancellationToken).ConfigureAwait(false);
            await reader.ReadAsStartObjectAsync(cancellationToken).ConfigureAwait(false);
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
            await reader.ReadAsPropertyNameAsync(fieldName, cancellationToken).ConfigureAwait(false);
            return await reader.ReadAsVersionAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
