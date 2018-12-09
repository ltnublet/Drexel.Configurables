using System;

namespace Drexel.Configurables.Tests.Common
{
    public class IntentionalTestException : Exception
    {
        public IntentionalTestException(string message, params object[] args)
            : base(message)
        {
            this.Arguments = args;
        }

        public object[] Arguments { get; }
    }
}
