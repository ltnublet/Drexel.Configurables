using System;
using System.Collections.Generic;
using System.Composition;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;
using Drexel.Configurables.Example.Contracts;
using Drexel.Configurables.External;

namespace Drexel.Configurables.Example.Local
{
    [Export(typeof(IExamplePlugin))]
    public class LocalPlugin : IExamplePlugin
    {
        static LocalPlugin()
        {
            ////LocalPlugin.DirectoryRequirement = Requirements.NewFilePath();
            ////LocalPlugin.IncludeSubfoldersRequirement = Requirements.NewBool();

            // TODO: replace these with real requirements
            LocalPlugin.DirectoryRequirement = new ClassRequirement<FilePath>(
                Guid.NewGuid(),
                RequirementTypes.FilePath);
            LocalPlugin.IncludeSubfoldersRequirement = new StructRequirement<bool>(
                Guid.NewGuid(),
                RequirementTypes.Boolean);

            LocalPlugin.RequirementSet = new RequirementSet(
                LocalPlugin.DirectoryRequirement,
                LocalPlugin.IncludeSubfoldersRequirement);
        }

        private static readonly ClassRequirement<FilePath> DirectoryRequirement;
        private static readonly StructRequirement<bool> IncludeSubfoldersRequirement;
        private static readonly RequirementSet RequirementSet;

        public LocalPlugin()
        {
            this.Name = "Local Plugin";
            this.Publisher = "Foo Flooferson";
            this.Version = new Version(8, 6, 7, 5);
            this.Id = Guid.Parse("2f7d1b01-2788-44c9-b88f-77e1f3f41d05");
        }

        public string Name { get; }

        public string Publisher { get; }

        public Version Version { get; }

        public Guid Id { get; }

        public RequirementSet Requirements => LocalPlugin.RequirementSet;

        public IPluginStartup CreateStartup(Configuration configuration)
        {
            return new LocalStartup(
                this,
                configuration.Get(LocalPlugin.DirectoryRequirement)
                    ?? throw new ArgumentNullException(nameof(LocalPlugin.DirectoryRequirement)),
                configuration.Get(LocalPlugin.IncludeSubfoldersRequirement));
        }
    }
}
