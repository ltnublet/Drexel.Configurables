using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Drexel.Configurables.Json.Exceptions;
using Newtonsoft.Json;

namespace Drexel.Configurables.Json
{
    internal sealed class JsonStreamReader : IDisposable
    {
        private readonly TextReader underlyingReader;
        private readonly JsonTextReader reader;
        private readonly bool leaveOpen;
        private bool isDisposed;

        public JsonStreamReader(
            Stream stream,
            bool leaveOpen,
            int bufferSize = 1024)
        {
            this.leaveOpen = leaveOpen;
            this.isDisposed = false;

            this.underlyingReader = new StreamReader(
                stream,
                Encoding.UTF8,
                false,
                bufferSize,
                leaveOpen);
            this.reader = new JsonTextReader(this.underlyingReader);
        }

        private object Value
        {
            get
            {
                this.ThrowIfDisposed();
                return this.reader.Value;
            }
        }

        private JsonToken TokenType
        {
            get
            {
                this.ThrowIfDisposed();
                return this.reader.TokenType;
            }
        }

        public void ReadAsStartObject()
        {
            this.ThrowIfDisposed();
            this.ReadAsTokenType(JsonToken.StartObject);
        }

        public void ReadAsEndObject()
        {
            this.ThrowIfDisposed();
            this.ReadAsTokenType(JsonToken.EndObject);
        }

        public void ReadAsArray(IReadOnlyDictionary<JsonToken, Func<JsonStreamReader, bool>> tokenCallbacks)
        {
            if (tokenCallbacks.ContainsKey(JsonToken.EndArray))
            {
                throw new JsonStreamReaderException(
                    "Internal exception.",
                    new ArgumentException("Callbacks cannot contain EndArray.", nameof(tokenCallbacks)));
            }

            this.ReadAsTokenType(JsonToken.StartArray);
            bool @continue = true;
            while (this.reader.Read() && @continue)
            {
                JsonToken type = this.reader.TokenType;
                if (type == JsonToken.EndArray)
                {
                    @continue = false;
                }
                else if (tokenCallbacks.TryGetValue(type, out Func<JsonStreamReader, bool> callback))
                {
                    @continue = callback.Invoke(this);
                }
                else
                {
                    throw new JsonStreamReaderException($"Unanticipated token type. Token type: '{type}'.");
                }
            }

            if (@continue)
            {
                throw new JsonStreamReaderException("Unexpected end of stream.");
            }
        }

        public string ReadAsString()
        {
            this.ThrowIfDisposed();
            return this.reader.ReadAsString();
        }

        public Version ReadAsVersion()
        {
            this.ThrowIfDisposed();
            string value = this.reader.ReadAsString();
            if (Version.TryParse(value, out Version result))
            {
                return result;
            }
            else
            {
                throw new JsonStreamReaderException("Failed to read value as a Version.");
            }
        }

        public Version ReadAsVersion(string propertyName)
        {
            this.ReadAsPropertyName(propertyName);
            return this.ReadAsVersion();
        }

        public Guid ReadAsGuid()
        {
            this.ThrowIfDisposed();
            string value = this.reader.ReadAsString();
            if (Guid.TryParse(value, out Guid result))
            {
                return result;
            }
            else
            {
                throw new JsonStreamReaderException("Failed to read value as a Guid.");
            }
        }

        public Guid ReadAsGuid(string propertyName)
        {
            this.ReadAsPropertyName(propertyName);
            return this.ReadAsGuid();
        }

        public string ReadAsPropertyName()
        {
            this.ThrowIfDisposed();
            this.ReadAsTokenType(JsonToken.PropertyName);
            if (this.reader.Value is string asString)
            {
                return asString;
            }
            else
            {
                throw new JsonStreamReaderException("Failed to read value as a property name.");
            }
        }

        public void ReadAsPropertyName(string propertyName)
        {
            string actual = this.ReadAsPropertyName();
            if (!StringComparer.OrdinalIgnoreCase.Equals(propertyName, actual))
            {
                throw new JsonStreamReaderException(
                    $"Failed to read expected property name. Expected property name: '{propertyName}', actual property name: '{actual}'.");
            }
        }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                if (!this.leaveOpen)
                {
                    this.underlyingReader.Dispose();
                }
            }
        }

        [System.Diagnostics.DebuggerHidden]
        private void ReadAsTokenType(JsonToken expected)
        {
            try
            {
                if (!this.reader.Read())
                {
                    throw new JsonStreamReaderException(
                        "Unexpected end of stream.");
                }

                if (this.reader.TokenType != expected)
                {
                    throw new JsonStreamReaderException(
                        $"Failed to read expected token type. Expected token type: '{expected}', actual token type: '{this.reader.TokenType}'.");
                }
            }
            catch (Exception e)
            {
                throw new JsonStreamReaderException(
                    $"Failed to read expected token type. Expected token type: '{expected}'.",
                    e);
            }
        }

        [System.Diagnostics.DebuggerHidden]
        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(nameof(JsonStreamReader));
            }
        }
    }
}
