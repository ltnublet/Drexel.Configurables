using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Persistables.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Drexel.Configurables.Persistables.Json
{
    public sealed class JsonRestorer : IRestorer, IDisposable
    {
        private readonly Stream stream;
        private readonly bool leaveOpen;

        public JsonRestorer(Stream stream, bool leaveOpen = true)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Nothing to do, but we're reserving the interface for now.
        }

        public IPersistableConfiguration Restore(int millisecondsTimeout = -1)
        {
            Task<IPersistableConfiguration> task = this.RestoreAsync();
            task.Wait(millisecondsTimeout);
            return task.Result;
        }

        public async Task<IPersistableConfiguration> RestoreAsync(CancellationToken token = default(CancellationToken))
        {
            using (StreamReader underlyingReader = new StreamReader(
                this.stream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: this.leaveOpen))
            using (JsonReader reader = new JsonTextReader(underlyingReader))
            {
                await reader.ReadAsync(token).ConfigureAwait(false); // {
                await reader.ReadAsync(token).ConfigureAwait(false); // "Version":
                Version version = Version.Parse(
                    await reader.ReadAsStringAsync(token).ConfigureAwait(false)); // "x.x.x.x",

                if (version == new Version("1.0.0.0"))
                {
                    new PersistableConfiguration()
                }
                else
                {
                    throw new NotImplementedException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Support for file version '{0}' is not implemented.",
                            version.ToString()));
                }
            }
        }
    }
}
