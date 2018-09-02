using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Tests.Common.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.External.Tests
{
    [TestClass]
    public class JsonWriterTests
    {
        [TestMethod]
        public void JsonWriter_Ctor_Succeeds()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                JsonWriter writer = new JsonWriter(stream);

                Assert.IsNotNull(writer);
            }
        }

        [TestMethod]
        public void JsonWriter_Ctor_PropagatesParameters()
        {
            bool autoFlush = true;
            bool pretty = true;
            string newLine = "foo";
            Encoding encoding = ASCIIEncoding.ASCII;
            CultureInfo culture = CultureInfo.CurrentCulture;
            TaskScheduler scheduler = TaskScheduler.Default;

            using (MemoryStream stream = new MemoryStream())
            {
                JsonWriter writer = new JsonWriter(
                    stream,
                    autoFlush: autoFlush,
                    pretty: pretty,
                    newLine: newLine,
                    encoding: encoding,
                    culture: culture,
                    scheduler: scheduler);

                Assert.AreEqual(autoFlush, writer.State.AutoFlush);
                Assert.AreSame(culture, writer.State.Culture);
                Assert.AreSame(encoding, writer.State.Encoding);
                Assert.AreSame(newLine, writer.State.NewLine);
                Assert.AreEqual(pretty, writer.State.Pretty);
                Assert.AreSame(scheduler, writer.State.Scheduler);
            }
        }

        [TestMethod]
        public void JsonWriter_Ctor_NullStream_ThrowsArgumentNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new JsonWriter(null));
        }

        [TestMethod]
        public void JsonWriter_Ctor_UnwritableStream_ThrowsArgument()
        {
            Assert.ThrowsException<ArgumentException>(() => new JsonWriter(new TestStream(canWrite: false)));
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task JsonWriter_Write_AutoFlushEnabled_Flushes(bool autoFlush)
        {
            TestStream stream = new TestStream();
            JsonWriter writer = new JsonWriter(stream, autoFlush: autoFlush);
            await writer.WriteObjectStart(CancellationToken.None);
            Assert.IsFalse(stream.Flushed);
            await writer.WriteObjectEnd(CancellationToken.None);
            Assert.AreEqual(autoFlush, stream.Flushed);
        }

        [TestMethod]
        public async Task JsonWriter_WriteObjectStart()
        {
            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream))
            {
                await writer.WriteObjectStart(CancellationToken.None);

                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();

                Assert.AreEqual("{", content);
            }
        }

        [TestMethod]
        public async Task JsonWriter_WriteObjectEnd()
        {
            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream))
            {
                await writer.WriteObjectEnd(CancellationToken.None);

                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();

                Assert.AreEqual("}", content);
            }
        }

        [TestMethod]
        public async Task JsonWriter_WriteArrayStart()
        {
            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream))
            {
                await writer.WriteArrayStart(CancellationToken.None);

                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();

                Assert.AreEqual("[", content);
            }
        }

        [TestMethod]
        public async Task JsonWriter_WriteArrayEnd()
        {
            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream))
            {
                await writer.WriteArrayEnd(CancellationToken.None);

                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();

                Assert.AreEqual("]", content);
            }
        }

        [TestMethod]
        public async Task JsonWriter_WriteNull()
        {
            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream))
            {
                await writer.WriteNull(CancellationToken.None);

                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();

                Assert.AreEqual("null", content);
            }
        }

        [DataTestMethod]
        [DataRow(false, "ABC123xyz", "\"ABC123xyz\":")]
        [DataRow(true, "ABC123xyz", "\"ABC123xyz\": ")]
        public async Task JsonWriter_WritePropertyName(bool pretty, string input, string expected)
        {
            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream, pretty: pretty))
            {
                await writer.WritePropertyName(input, CancellationToken.None);

                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();

                Assert.AreEqual(expected, content);
            }
        }

        [TestMethod]
        public async Task JsonWriter_WritePropertyName_WritesNullWhenNameIsNull()
        {
            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream))
            {
                await writer.WritePropertyName(null, CancellationToken.None);

                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();

                Assert.AreEqual("null", content);
            }
        }

        [TestMethod]
        public async Task JsonWriter_WriteValue_WritesNullWhenValueIsNull()
        {
            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream))
            {
                await writer.WriteValue(null, CancellationToken.None);

                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();

                Assert.AreEqual("null", content);
            }
        }

        [TestMethod]
        public async Task JsonWriter_WriteValue()
        {
            const string value = "ValueWritten123";

            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream))
            {
                await writer.WriteValue(value, CancellationToken.None);

                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();

                Assert.AreEqual($"\"{value}\"", content);
            }
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task JsonWriter_Pretty_Honored(bool pretty)
        {
            using (MemoryStream stream = new MemoryStream())
            using (JsonWriter writer = new JsonWriter(stream, pretty: pretty))
            {
                CancellationToken token = CancellationToken.None;

                await writer.WriteObjectStart(token);
                await writer.WritePropertyName("Foo", token);
                await writer.WriteValue("bar", token);
                await writer.WritePropertyName("Baz", token);
                await writer.WriteValue("bang", token);
                await writer.WritePropertyName("ArrayProp", token);
                await writer.WriteArrayStart(token);
                await writer.WriteValue("1", token);
                await writer.WriteValue("2", token);
                await writer.WriteValue("3", token);
                await writer.WriteArrayEnd(token);
                await writer.WritePropertyName("LastProperty", token);
                await writer.WriteValue("doodle", token);
                await writer.WriteObjectEnd(token);

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                Assert.IsFalse(string.IsNullOrWhiteSpace(content));
            }
        }

        private class TestStream : Stream
        {
            private readonly bool canWrite;
            private long length;

            public TestStream(bool canWrite = true)
            {
                this.length = 0L;
                this.canWrite = canWrite;
            }

            public override bool CanRead => true;

            public override bool CanSeek => true;

            public override bool CanWrite => this.canWrite;

            public override long Length => this.length;

            public override long Position { get; set; } = 0L;

            public bool Flushed { get; private set; }

            public override void Flush()
            {
                this.Flushed = true;
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                this.Flushed = true;
                return Task.CompletedTask;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        this.Position = offset;
                        break;
                    case SeekOrigin.End:
                        this.Position = this.Length + offset;
                        break;
                    case SeekOrigin.Current:
                        this.Position = this.Position + offset;
                        break;
                    default:
                        // Do nothing.
                        break;
                }

                return this.Position;
            }

            public override void SetLength(long value)
            {
                this.length = value;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                // Do nothing.
            }

            public override Task WriteAsync(
                byte[] buffer,
                int offset,
                int count,
                CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}