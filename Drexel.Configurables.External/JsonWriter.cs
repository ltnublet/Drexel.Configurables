using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.External.Internals;

namespace Drexel.Configurables.External
{
    /// <summary>
    /// Performs write operations to a supplied <see cref="Stream"/>.
    /// </summary>
    public sealed class JsonWriter : IDisposable
    {
        /// <summary>
        /// Contains the internal state of the <see cref="JsonWriter"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.MaintainabilityRules",
            "SA1401:Fields must be private",
            Justification = "Intentional.")]
        internal readonly JsonWriterState State;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class.
        /// </summary>
        /// <param name="stream">
        /// The underlying <see cref="Stream"/> to write to.
        /// </param>
        /// <param name="autoFlush">
        /// When <see langword="true"/>, the underlying <see cref="Stream"/> <paramref name="stream"/> will be flushed
        /// (via <see cref="Stream.FlushAsync(CancellationToken)"/>) as part of calls to
        /// <see cref="JsonWriter.WriteObjectEnd(CancellationToken)"/>.
        /// </param>
        /// <param name="pretty">
        /// When <see langword="true"/>, write operations will include additional whitespace to increase
        /// human-readability.
        /// </param>
        /// <param name="newLine">
        /// The <see langword="string"/> used for whitespace newlines. When <see langword="null"/>,
        /// <see cref="Environment.NewLine"/> will be used.
        /// </param>
        /// <param name="encoding">
        /// The <see cref="Encoding"/> to use when writing characters to the <see cref="Stream"/>. When
        /// <see langword="null"/>", <see cref="Encoding.UTF8"/> will be used.
        /// </param>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> to use when performing write operations. When <see langword="null"/>,
        /// <see cref="CultureInfo.InvariantCulture"/> will be used.
        /// </param>
        /// <param name="scheduler">
        /// The <see cref="TaskScheduler"/> to schedule <see langword="async"/> writes on. When <see langword="null"/>,
        /// <see cref="TaskScheduler.Current"/> will be used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Occurs when a supplied argument is illegally <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Occurs when the <see cref="Stream.CanWrite"/> property of the <see cref="Stream"/>
        /// <paramref name="stream"/> does not return <see langword="true"/>.
        /// </exception>
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

        /// <summary>
        /// Disposes of the <see cref="JsonWriter"/>.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.State.Dispose();
                this.disposed = true;
            }
        }

        /// <summary>
        /// Writes the start of an object to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="token">
        /// The <see cref="CancellationToken"/> that will be assigned to the write operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the write operation.
        /// </returns>
        public Task WriteObjectStart(CancellationToken token)
        {
            return this.WriteToStream(
                "{",
                this.State.NoChange,
                this.State.WroteObjectOrArrayStart,
                token);
        }

        /// <summary>
        /// Writes the end of an object to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="token">
        /// The <see cref="CancellationToken"/> that will be assigned to the write operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the write operation.
        /// </returns>
        public Task WriteObjectEnd(CancellationToken token)
        {
            return this.WriteToStream(
                "}",
                this.State.WritingObjectEnd,
                this.State.WroteObjectOrArrayEnd,
                token);
        }

        /// <summary>
        /// Writes the start of an array to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="token">
        /// The <see cref="CancellationToken"/> that will be assigned to the write operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the write operation.
        /// </returns>
        public Task WriteArrayStart(CancellationToken token)
        {
            return this.WriteToStream(
                "[",
                this.State.NoChange,
                this.State.WroteObjectOrArrayStart,
                token);
        }

        /// <summary>
        /// Writes the end of an array to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="token">
        /// The <see cref="CancellationToken"/> that will be assigned to the write operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the write operation.
        /// </returns>
        public Task WriteArrayEnd(CancellationToken token)
        {
            return this.WriteToStream(
                "]",
                this.State.WritingArrayEnd,
                this.State.WroteObjectOrArrayEnd,
                token);
        }

        /// <summary>
        /// Writes the <see langword="string"/> <paramref name="value"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="value">
        /// The <see langword="string"/> to write to the stream.
        /// </param>
        /// <param name="token">
        /// The <see cref="CancellationToken"/> that will be assigned to the write operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the write operation.
        /// </returns>
        public Task WriteValue(string value, CancellationToken token)
        {
            if (value == null)
            {
                return this.WriteNull(token);
            }

            return this.WriteValueInternal(
                string.Format(
                    this.State.Culture,
                    "\"{0}\"",
                    value.JsonEscape()),
                token);
        }

        /// <summary>
        /// Writes the <see langword="object"/> <paramref name="value"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="value">
        /// The <see langword="object"/> to write to the stream.
        /// </param>
        /// <param name="token">
        /// The <see cref="CancellationToken"/> that will be assigned to the write operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the write operation.
        /// </returns>
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

        /// <summary>
        /// Writes <see langword="null"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="token">
        /// The <see cref="CancellationToken"/> that will be assigned to the write operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the write operation.
        /// </returns>
        public Task WriteNull(CancellationToken token)
        {
            return this.WriteNullInternal(
                this.State.NoChange,
                this.State.WroteValue,
                token);
        }

        /// <summary>
        /// Writes the property name <paramref name="name"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="name">
        /// The property name to write to the <see cref="Stream"/>.
        /// </param>
        /// <param name="token">
        /// The <see cref="CancellationToken"/> that will be assigned to the write operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the write operation.
        /// </returns>
        public Task WritePropertyName(string name, CancellationToken token)
        {
            if (name == null)
            {
                return this.WriteNullInternal(
                    this.State.NoChange,
                    this.State.WroteName,
                    token);
            }

            string escaped = name.JsonEscape();

            return this.WriteToStream(
                string.Format(
                    this.State.Culture,
                    this.State.Pretty
                        ? "\"{0}\": "
                        : "\"{0}\":",
                    name.JsonEscape()),
                this.State.NoChange,
                this.State.WroteName,
                token);
        }

        private Task WriteNullInternal(
            Action precedingStateTransform,
            Action succeedingStateTransform,
            CancellationToken token)
        {
            const string @null = "null";
            return this.WriteToStream(
                @null,
                precedingStateTransform,
                succeedingStateTransform,
                token);
        }

        private Task WriteValueInternal(string value, CancellationToken token)
        {
            return this.WriteToStream(
                value,
                this.State.NoChange,
                this.State.WroteValue,
                token);
        }

        private Task WriteToStream(
            string assumedSafeValue,
            Action precedingStateTransform,
            Action succeedingStateTransform,
            CancellationToken token)
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

                            operation.PrecedingStateTransform.Invoke();

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

                            builder.Append(operation.Content);

                            byte[] bytes = operation.State.Encoding.GetBytes(builder.ToString());
                            await operation.State.Stream
                                .WriteAsync(
                                    bytes,
                                    0,
                                    bytes.Length,
                                    operation.Token)
                                .ConfigureAwait(false);

                            operation.SucceedingStateTransform.Invoke();

                            if (operation.State.AutoFlush && operation.State.CurrentTokenRequiresFlush)
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
                        precedingStateTransform,
                        succeedingStateTransform,
                        token),
                    token,
                    TaskCreationOptions.None,
                    this.State.Scheduler);
        }

        /// <summary>
        /// Represents the internal state of the <see cref="JsonWriter"/>.
        /// </summary>
        internal sealed class JsonWriterState : IDisposable
        {
#pragma warning disable SA1401 // Fields must be private
            /// <summary>
            /// The underlying stream being operated on.
            /// </summary>
            public readonly Stream Stream;

            /// <summary>
            /// Whether autoflush is enabled.
            /// </summary>
            public readonly bool AutoFlush;

            /// <summary>
            /// Whether pretty printing is enabled.
            /// </summary>
            public readonly bool Pretty;

            /// <summary>
            /// The string to use for newlines when pretty printing.
            /// </summary>
            public readonly string NewLine;

            /// <summary>
            /// The <see cref="Encoding"/> to use when converting user-supplied <see langword="string"/>s to
            /// <see langword="T:Byte[]"/> while writing to the <see cref="Stream"/>.
            /// </summary>
            public readonly Encoding Encoding;

            /// <summary>
            /// The <see cref="CultureInfo"/> to use when calling
            /// <see cref="string.Format(IFormatProvider, string, object[])"/> while escaping user-supplied
            /// <see langword="string"/>s.
            /// </summary>
            public readonly CultureInfo Culture;

            /// <summary>
            /// The <see cref="TaskScheduler"/> to use when scheduling <see cref="Task"/>s for writing to the
            /// <see cref="Stream"/>.
            /// </summary>
            public readonly TaskScheduler Scheduler;

            /// <summary>
            /// The current depth (in spaces) when pretty printing.
            /// </summary>
            public int Depth;

            /// <summary>
            /// The thread-safe lock to use when writing to the <see cref="Stream"/>.
            /// </summary>
            public SemaphoreSlim WriterLock;

            /// <summary>
            /// Indicates whether the current token requires a flush to occur, if <see cref="Pretty"/> is
            /// <see langword="true"/>.
            /// </summary>
            public bool CurrentTokenRequiresFlush;

            /// <summary>
            /// Indicates whether the previous token was a property name.
            /// </summary>
            public bool PreviousTokenWasName;

            /// <summary>
            /// Indicates whether the previous token was the start of an object or array.
            /// </summary>
            public bool PreviousTokenWasObjectOrArrayStart;

            /// <summary>
            /// Indicates whether the previous token was the end of an objecy or array.
            /// </summary>
            public bool PreviousTokenWasObjectOrArrayEnd;

            /// <summary>
            /// Indicates whether the previous token requires a comma to be written when performing the next write.
            /// </summary>
            public bool PreviousTokenRequiresComma;

            /// <summary>
            /// Indicates whether the previous token requires a newline to be written when performing the next write.
            /// </summary>
            public bool PreviousTokenRequiresNewLine;
#pragma warning restore SA1401 // Fields must be private

            /// <summary>
            /// Initializes a new instance of the <see cref="JsonWriterState"/> class.
            /// </summary>
            /// <param name="stream">
            /// The <see cref="Stream"/>.
            /// </param>
            /// <param name="autoFlush">
            /// Indicates whether auto-flush is enabled, which is flush the underlying <see cref="Stream"/> whenever
            /// the end of an object is written to the stream.
            /// </param>
            /// <param name="pretty">
            /// Indicates whether pretty printing is enabled.
            /// </param>
            /// <param name="newLine">
            /// The newline <see langword="string"/> to use when pretty printing.
            /// </param>
            /// <param name="encoding">
            /// The <see cref="Encoding"/>.
            /// </param>
            /// <param name="culture">
            /// The <see cref="CultureInfo"/>.
            /// </param>
            /// <param name="scheduler">
            /// The <see cref="TaskScheduler"/>.
            /// </param>
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
                this.CurrentTokenRequiresFlush = false;
                this.PreviousTokenWasName = false;
                this.PreviousTokenWasObjectOrArrayStart = false;
                this.PreviousTokenWasObjectOrArrayEnd = false;
                this.PreviousTokenRequiresComma = false;
                this.PreviousTokenRequiresNewLine = false;
            }

            /// <summary>
            /// Mutates the state to indicate that the previous write operation was for an object or array start.
            /// </summary>
            public void WroteObjectOrArrayStart()
            {
                this.PreviousTokenWasName = false;
                this.PreviousTokenWasObjectOrArrayStart = true;
                this.PreviousTokenWasObjectOrArrayEnd = false;
                this.PreviousTokenRequiresComma = false;
                this.PreviousTokenRequiresNewLine = true;
                this.Depth++;
            }

            /// <summary>
            /// Mutates the state to indicate that the current write operation is for an object end.
            /// </summary>
            public void WritingObjectEnd()
            {
                this.CurrentTokenRequiresFlush = true;
                this.PreviousTokenRequiresComma = false;
                this.Depth--;
            }

            /// <summary>
            /// Mutates the state to indicate that the current write operation is for an array end.
            /// </summary>
            public void WritingArrayEnd()
            {
                this.PreviousTokenRequiresComma = false;
                this.Depth--;
            }

            /// <summary>
            /// Mutates the state to indicate that the previous write operation was for an array end.
            /// </summary>
            public void WroteObjectOrArrayEnd()
            {
                this.CurrentTokenRequiresFlush = false;
                this.PreviousTokenWasName = false;
                this.PreviousTokenWasObjectOrArrayStart = false;
                this.PreviousTokenWasObjectOrArrayEnd = true;
                this.PreviousTokenRequiresComma = true;
                this.PreviousTokenRequiresNewLine = true;
            }

            /// <summary>
            /// Mutates the state to indicate that the previous operation was for a property name.
            /// </summary>
            public void WroteName()
            {
                this.PreviousTokenWasName = true;
                this.PreviousTokenWasObjectOrArrayStart = false;
                this.PreviousTokenWasObjectOrArrayEnd = false;
                this.PreviousTokenRequiresComma = false;
                this.PreviousTokenRequiresNewLine = false;
            }

            /// <summary>
            /// Mutates the state to indicate that the previous operation was for a value.
            /// </summary>
            public void WroteValue()
            {
                this.PreviousTokenWasName = false;
                this.PreviousTokenWasObjectOrArrayStart = false;
                this.PreviousTokenWasObjectOrArrayEnd = false;
                this.PreviousTokenRequiresComma = true;
                this.PreviousTokenRequiresNewLine = true;
            }

            /// <summary>
            /// Retains the current state.
            /// </summary>
            public void NoChange()
            {
                // Do nothing.
            }

            /// <summary>
            /// Disposes of the state.
            /// </summary>
            public void Dispose()
            {
                this.WriterLock.Dispose();
            }
        }

        private class JsonWriteOperation
        {
#pragma warning disable SA1401 // Fields must be private
            public readonly JsonWriterState State;
            public readonly string Content;
            public readonly Action PrecedingStateTransform;
            public readonly Action SucceedingStateTransform;
            public readonly CancellationToken Token;
#pragma warning restore SA1401 // Fields must be private

            public JsonWriteOperation(
                JsonWriterState state,
                string content,
                Action precedingStateTransform,
                Action succeedingStateTransform,
                CancellationToken token)
            {
                this.State = state;
                this.Content = content;
                this.PrecedingStateTransform = precedingStateTransform;
                this.SucceedingStateTransform = succeedingStateTransform;
                this.Token = token;
            }
        }
    }
}
