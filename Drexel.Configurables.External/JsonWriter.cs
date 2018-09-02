using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.External.Internals;

namespace Drexel.Configurables.External
{
    public sealed class JsonWriter : IDisposable
    {
        internal readonly JsonWriterState State;
        private bool disposed;

        public JsonWriter(
            Stream stream,
            bool autoFlush = false,
            bool pretty = false,
            string newLine = null,
            Encoding encoding = null,
            CultureInfo culture = null,
            TaskScheduler scheduler = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            else if (!stream.CanWrite)
            {
                throw new ArgumentException(
                    "`stream.CanWrite` returned false; `JsonWriter` requires a writable stream.");
            }

            this.State = new JsonWriterState(
                stream,
                autoFlush,
                pretty,
                newLine ?? Environment.NewLine,
                encoding ?? Encoding.UTF8,
                culture ?? CultureInfo.InvariantCulture,
                scheduler ?? TaskScheduler.Current);

            this.disposed = false;
        }

        public Task WriteObjectStart(CancellationToken token)
        {
            return this.WriteToStream(
                "{",
                this.State.WroteObjectOrArrayStart,
                token);
        }

        public Task WriteObjectEnd(CancellationToken token)
        {
            return this.WriteToStream(
                "}",
                this.State.WroteObjectOrArrayEnd,
                token,
                true);
        }

        public Task WriteArrayStart(CancellationToken token)
        {
            return this.WriteToStream(
                "[",
                this.State.WroteObjectOrArrayStart,
                token);
        }

        public Task WriteArrayEnd(CancellationToken token)
        {
            return this.WriteToStream(
                "]",
                this.State.WroteObjectOrArrayEnd,
                token,
                true);
        }

        public Task WriteValue(string value, CancellationToken token)
        {
            if (value == null)
            {
                return this.WriteNull(token);
            }

            return this.WriteValueInternal(
                $"\"{value.JsonEscape()}\"",
                token);
        }

        public Task WriteValue(object value, CancellationToken token)
        {
            if (value == null)
            {
                return this.WriteNull(token);
            }
            else if (value is string asString)
            {
                return this.WriteValue(value as string, token);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Task WriteNull(CancellationToken token)
        {
            return this.WriteNullInternal(this.State.WroteValue, token);
        }

        public Task WritePropertyName(string name, CancellationToken token)
        {
            if (name == null)
            {
                return this.WriteNullInternal(this.State.WroteName, token);
            }

            string escaped = name.JsonEscape();

            return this.WriteToStream(
                string.Format(
                    this.State.Culture,
                    this.State.Pretty
                        ? "\"{0}\": "
                        : "\"{0}\":",
                    name.JsonEscape()),
                this.State.WroteName,
                token);
        }

        private Task WriteNullInternal(Action stateTransform, CancellationToken token)
        {
            const string @null = "null";
            return this.WriteToStream(
                @null,
                stateTransform,
                token);
        }

        private Task WriteValueInternal(string value, CancellationToken token)
        {
            return this.WriteToStream(
                value,
                this.State.WroteValue,
                token);
        }

        private Task WriteToStream(
            string assumedSafeValue,
            Action stateTransform,
            CancellationToken token,
            bool closingToken = false)
        {
            return Task.Factory
                .StartNew(
                    async asObject =>
                    {
                        JsonWriteOperation operation = (JsonWriteOperation)asObject;

                        await operation.State.WriterLock.WaitAsync(operation.Token).ConfigureAwait(false);

                        try
                        {
                            StringBuilder builder;
                            int depthInSpaces = -1;

                            if (operation.ClosingToken)
                            {
                                operation.StateTransform.Invoke();
                            }

                            if (operation.State.Pretty)
                            {
                                depthInSpaces = operation.State.Depth * 4;
                                builder = new StringBuilder(
                                    operation.Content.Length
                                        + operation.State.NewLine.Length
                                        + 1
                                        + depthInSpaces);
                            }
                            else
                            {
                                builder = new StringBuilder(operation.Content.Length + 1);
                            }

                            if (!operation.ClosingToken && operation.State.PreviousTokenRequiresComma)
                            {
                                builder.Append(',');
                            }

                            if (operation.State.Pretty && operation.State.PreviousTokenRequiresNewLine)
                            {
                                builder.Append(operation.State.NewLine);
                                if (depthInSpaces > 0)
                                {
                                    builder.Append(new string(' ', depthInSpaces));
                                }
                            }

                            builder.Append(operation.Content);

                            byte[] bytes = operation.State.Encoding.GetBytes(builder.ToString());
                            await operation.State.Stream
                                .WriteAsync(
                                    bytes,
                                    0,
                                    bytes.Length,
                                    operation.Token)
                                .ConfigureAwait(false);

                            if (!operation.ClosingToken)
                            {
                                operation.StateTransform.Invoke();
                            }

                            if (operation.State.AutoFlush && operation.State.Depth == 0)
                            {
                                await operation
                                    .State
                                    .Stream
                                    .FlushAsync()
                                    .ConfigureAwait(false);
                            }
                        }
                        finally
                        {
                            operation.State.WriterLock.Release();
                        }
                    },
                    new JsonWriteOperation(
                        this.State,
                        assumedSafeValue,
                        stateTransform,
                        closingToken,
                        token),
                    token,
                    TaskCreationOptions.None,
                    this.State.Scheduler);
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.State.Dispose();
                this.disposed = true;
            }
        }

        internal class JsonWriterState : IDisposable
        {
            public readonly Stream Stream;
            public readonly bool AutoFlush;
            public readonly bool Pretty;
            public readonly string NewLine;
            public readonly Encoding Encoding;
            public readonly CultureInfo Culture;
            public readonly TaskScheduler Scheduler;

            public int Depth;
            public SemaphoreSlim WriterLock;
            public bool PreviousTokenWasName;
            public bool PreviousTokenWasObjectOrArrayStart;
            public bool PreviousTokenWasObjectOrArrayEnd;
            public bool PreviousTokenRequiresComma;
            public bool PreviousTokenRequiresNewLine;

            public JsonWriterState(
                Stream stream,
                bool autoFlush,
                bool pretty,
                string newLine,
                Encoding encoding,
                CultureInfo culture,
                TaskScheduler scheduler)
            {
                this.Stream = stream;
                this.AutoFlush = autoFlush;
                this.Pretty = pretty;
                this.NewLine = newLine;
                this.Encoding = encoding;
                this.Culture = culture;
                this.Scheduler = scheduler;

                this.Depth = 0;
                this.WriterLock = new SemaphoreSlim(1);
                this.PreviousTokenWasName = false;
                this.PreviousTokenWasObjectOrArrayStart = false;
                this.PreviousTokenWasObjectOrArrayEnd = false;
                this.PreviousTokenRequiresComma = false;
                this.PreviousTokenRequiresNewLine = false;
            }

            public void WroteObjectOrArrayStart()
            {
                this.PreviousTokenWasName = false;
                this.PreviousTokenWasObjectOrArrayStart = true;
                this.PreviousTokenWasObjectOrArrayEnd = false;
                this.PreviousTokenRequiresComma = false;
                this.PreviousTokenRequiresNewLine = true;
                this.Depth++;
            }

            public void WroteObjectOrArrayEnd()
            {
                this.PreviousTokenWasName = false;
                this.PreviousTokenWasObjectOrArrayStart = false;
                this.PreviousTokenWasObjectOrArrayEnd = true;
                this.PreviousTokenRequiresComma = true;
                this.PreviousTokenRequiresNewLine = true;
                this.Depth--;
            }

            public void WroteName()
            {
                this.PreviousTokenWasName = true;
                this.PreviousTokenWasObjectOrArrayStart = false;
                this.PreviousTokenWasObjectOrArrayEnd = false;
                this.PreviousTokenRequiresComma = false;
                this.PreviousTokenRequiresNewLine = false;
            }

            public void WroteValue()
            {
                this.PreviousTokenWasName = false;
                this.PreviousTokenWasObjectOrArrayStart = false;
                this.PreviousTokenWasObjectOrArrayEnd = false;
                this.PreviousTokenRequiresComma = true;
                this.PreviousTokenRequiresNewLine = true;
            }

            public void Dispose()
            {
                this.WriterLock.Dispose();
            }
        }

        private class JsonWriteOperation
        {
            public readonly JsonWriterState State;
            public readonly string Content;
            public readonly Action StateTransform;
            public readonly bool ClosingToken;
            public readonly CancellationToken Token;

            public JsonWriteOperation(
                JsonWriterState state,
                string content,
                Action stateTransform,
                bool closingToken,
                CancellationToken token)
            {
                this.State = state;
                this.Content = content;
                this.StateTransform = stateTransform;
                this.ClosingToken = closingToken;
                this.Token = token;
            }
        }
    }
}
