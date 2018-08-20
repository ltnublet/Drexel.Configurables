using System;

namespace Drexel.Configurables.Persistables.Contracts
{
    public sealed class PersistResult
    {
        public PersistResult(
            int exitCode,
            Exception exception = null)
        {
            this.ExitCode = exitCode;
            this.Exception = exception;
        }

        public int ExitCode { get; }

        public bool Excepted => this.Exception != null;

        public Exception Exception { get; }
    }
}
