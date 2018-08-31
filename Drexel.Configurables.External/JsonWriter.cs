using Drexel.Configurables.External.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.External
{
    public sealed class JsonWriter
    {
        private readonly JsonWriterState state;

        public JsonWriter(
            Stream stream,
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

            this.state = new JsonWriterState(
                stream,
                pretty,
                newLine ?? Environment.NewLine,
                encoding ?? Encoding.UTF8,
                culture ?? CultureInfo.InvariantCulture,
                scheduler ?? TaskScheduler.Current);
        }

        public Task WriteObjectStart(CancellationToken token)
        {
            return this.WriteToStream(
                "{",
                this.state.WroteObjectOrArrayStart,
                token);
        }

        public Task WriteObjectEnd(CancellationToken token)
        {
            return this.WriteToStream(
                "}",
                this.state.WroteObjectOrArrayEnd,
                token);
        }

        public Task WriteArrayStart(CancellationToken token)
        {
            return this.WriteToStream(
                "[",
                this.state.WroteObjectOrArrayStart,
                token);
        }

        public Task WriteArrayEnd(CancellationToken token)
        {
            return this.WriteToStream(
                "]",
                this.state.WroteObjectOrArrayEnd,
                token);
        }

        public Task WriteValue(string value, CancellationToken token)
        {
            if (value == null)
            {
                return this.WriteNull(token);
            }

            return this.WriteToStream(
                value.JsonEscape(),
                this.state.WroteValue,
                token);
        }

        public Task WriteNull(CancellationToken token)
        {
            return this.WriteNullInternal(this.state.WroteValue, token);
        }

        public Task WritePropertyName(string name, CancellationToken token)
        {
            if (name == null)
            {
                return this.WriteNullInternal(this.state.WroteName, token);
            }

            string escaped = name.JsonEscape();

            return this.WriteToStream(
                string.Format(
                    this.state.Culture,
                    this.state.Pretty
                        ? "\"{0}\": "
                        : "\"{0}\":",
                    name.JsonEscape()),
                this.state.WroteName,
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

        private Task WriteToStream(
            string assumedSafeValue,
            Action stateTransform,
            CancellationToken token)
        {
            const int maximumAdditionalCharacters = 4;

            return Task.Factory
                .StartNew(
                    async asObject =>
                    {
                        JsonWriteOperation operation = (JsonWriteOperation)asObject;

                        Monitor.Enter(operation.State.WriterLock);

                        StringBuilder builder;
                        int depthInSpaces = -1;

                        if (operation.State.Pretty)
                        {
                            depthInSpaces = operation.State.Depth * 4;
                            builder = new StringBuilder(
                                operation.Content.Length
                                    + maximumAdditionalCharacters
                                    + depthInSpaces);
                        }
                        else
                        {
                            builder = new StringBuilder(
                                operation.Content.Length + maximumAdditionalCharacters);
                        }

                        if (operation.State.PreviousTokenRequiresComma)
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

                        byte[] bytes = operation.State.Encoding.GetBytes(builder.ToString());
                        await operation.State.Stream
                            .WriteAsync(
                                bytes,
                                0,
                                bytes.Length,
                                operation.Token)
                            .ContinueWith(
                                x =>
                                {
                                    JsonWriteOperation nestedOperation = (JsonWriteOperation)x.AsyncState;
                                    nestedOperation.StateTransform.Invoke();
                                    Monitor.Exit(operation.State.WriterLock);
                                },
                                CancellationToken.None, // Do not allow the exit to be cancelled.
                                TaskContinuationOptions.ExecuteSynchronously,
                                operation.State.Scheduler)
                            .ConfigureAwait(false);
                    },
                    new JsonWriteOperation(
                        this.state,
                        assumedSafeValue,
                        stateTransform,
                        token),
                    token,
                    TaskCreationOptions.None,
                    this.state.Scheduler);
        }

        private class JsonWriterState
        {
            public readonly Stream Stream;
            public readonly bool Pretty;
            public readonly string NewLine;
            public readonly Encoding Encoding;
            public readonly CultureInfo Culture;
            public readonly TaskScheduler Scheduler;
            public int Depth;
            public object WriterLock;
            public bool PreviousTokenWasName;
            public bool PreviousTokenWasObjectOrArrayStart;
            public bool PreviousTokenWasObjectOrArrayEnd;
            public bool PreviousTokenRequiresComma;
            public bool PreviousTokenRequiresNewLine;

            public JsonWriterState(
                Stream stream,
                bool pretty,
                string newLine,
                Encoding encoding,
                CultureInfo culture,
                TaskScheduler scheduler)
            {
                this.Stream = stream;
                this.Pretty = pretty;
                this.NewLine = newLine;
                this.Encoding = encoding;
                this.Culture = culture;
                this.Scheduler = scheduler;

                this.Depth = 0;
                this.WriterLock = new object();
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
        }

        private class JsonWriteOperation
        {
            public readonly JsonWriterState State;
            public readonly string Content;
            public readonly Action StateTransform;
            public readonly CancellationToken Token;

            public JsonWriteOperation(
                JsonWriterState state,
                string content,
                Action stateTransform,
                CancellationToken token)
            {
                this.State = state;
                this.Content = content;
                this.StateTransform = stateTransform;
                this.Token = token;
            }
        }
    }
}
