using System;
using System.Collections.Generic;
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
            Encoding encoding = null,
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
                scheduler ?? TaskScheduler.Current,
                encoding ?? Encoding.UTF8)
            {
                WriterLock = new object(),
                Depth = 0,
                PreviousTokenWasName = false
            };
        }

        public Task WriteObjectStart(CancellationToken token)
        {
            // TODO need to do pretty printing?
            this.WriteToStream("{", token);
            throw new NotImplementedException();
        }

        public Task WriteObjectEnd()
        {
            throw new NotImplementedException();
        }

        public Task WriteArrayStart()
        {
            throw new NotImplementedException();
        }

        public Task WriteArrayEnd()
        {
            throw new NotImplementedException();
        }

        public Task WriteValue(object value)
        {
            throw new NotImplementedException();
        }

        public Task WritePropertyName(string name)
        {
            throw new NotImplementedException();
        }

        private Task WriteToStream(
            string assumedSafeValue,
            CancellationToken token)
        {
            return Task.Factory
                .StartNew(
                    async asObject =>
                    {
                        JsonWriteOperation operation = (JsonWriteOperation)asObject;
                        Monitor.Enter(operation.State.WriterLock);
                        byte[] bytes = operation.State.Encoding.GetBytes(assumedSafeValue);
                        await operation.State.Stream
                            .WriteAsync(
                                bytes,
                                0,
                                bytes.Length,
                                token)
                            .ContinueWith(
                                x => Monitor.Exit(operation.State.WriterLock),
                                CancellationToken.None, // Do not allow the exit to be cancelled.
                                TaskContinuationOptions.ExecuteSynchronously,
                                operation.State.Scheduler)
                            .ConfigureAwait(false);
                    },
                    new JsonWriteOperation(this.state, token),
                    token,
                    TaskCreationOptions.None,
                    this.state.Scheduler);
        }

        private class JsonWriterState
        {
            public readonly Stream Stream;
            public readonly TaskScheduler Scheduler;
            public readonly Encoding Encoding;
            public readonly CancellationToken Token;
            public int Depth;
            public object WriterLock;
            public bool PreviousTokenWasName;

            public JsonWriterState(
                Stream stream,
                TaskScheduler scheduler,
                Encoding encoding)
            {
                this.Stream = stream;
                this.Scheduler = scheduler;
                this.Encoding = encoding;
            }
        }

        private class JsonWriteOperation
        {
            public readonly JsonWriterState State;
            public readonly CancellationToken Token;

            public JsonWriteOperation(
                JsonWriterState state,
                CancellationToken token)
            {
                this.State = state;
                this.Token = token;
            }
        }
    }
}
