using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Example.Contracts
{
    public interface IExamplePlugin : IConfigurable
    {
        string Name { get; }

        string Publisher { get; }

        Version Version { get; }

        IPluginStartup CreateStartup(Configuration configuration);
    }
}
