using System;

namespace Drexel.Configurables.Json.Exceptions
{
    public class VersionNotSupportedException : JsonConfigurationRestorerException
    {
        public VersionNotSupportedException(Version unsupportedVersion)
            : base("The specified version is not supported.")
        {
            this.UnsupportedVersion = unsupportedVersion;
        }

        public Version UnsupportedVersion { get; }
    }
}
