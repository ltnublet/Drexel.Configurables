using System;
using System.Composition;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Example.Contracts;

namespace Drexel.Configurables.Example
{
    [Export(typeof(IExamplePlugin))]
    public class SystemPlugin : IExamplePlugin
    {
        public string Name => "System";

        public string Publisher => "Max Drexel";

        public Version Version { get; } = new Version(1, 0, 0, 0);

        public Guid Id { get; } = Guid.Parse("029bb74b-5728-4ba3-90f4-46ed0fe4e5ac");

        public RequirementSet Requirements => RequirementSet.Empty;

        public IPluginStartup CreateStartup(Configuration configuration) => new SystemStartup(this);
    }
}
