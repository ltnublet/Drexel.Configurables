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
                false,
                new ClassRequirementType<FilePath>(
                    Guid.NewGuid(),
                    (object? x, out FilePath? result) =>
                    {
                        if (x == null)
                        {
                            result = null;
                            return true;
                        }
                        else if (x is FilePath asPath)
                        {
                            result = asPath;
                            return true;
                        }
                        else
                        {
                            result = default;
                            return false;
                        }
                    },
                    (object? x, out IEnumerable<FilePath?>? result) =>
                    {
                        IEnumerable<FilePath?>? buffer = x as IEnumerable<FilePath?>?;

                        if (x == null)
                        {
                            result = null;
                            return true;
                        }
                        else if (buffer != null)
                        {
                            result = buffer;
                            return true;
                        }
                        else
                        {
                            result = default;
                            return false;
                        }
                    }));
            LocalPlugin.IncludeSubfoldersRequirement = new StructRequirement<bool>(
                Guid.NewGuid(),
                false,
                new StructRequirementType<bool>(
                    Guid.NewGuid(),
                    (object? x, out bool result) =>
                    {
                        if (x == null)
                        {
                            result = false;
                            return false;
                        }
                        else if (x is bool asBool)
                        {
                            result = asBool;
                            return true;
                        }
                        else
                        {
                            result = false;
                            return false;
                        }
                    },
                    (object? x, out IEnumerable<bool>? result) =>
                    {
                        IEnumerable<bool>? buffer = x as IEnumerable<bool>?;

                        if (x == null)
                        {
                            result = null;
                            return true;
                        }
                        else if (buffer != null)
                        {
                            result = buffer;
                            return true;
                        }
                        else
                        {
                            result = default;
                            return false;
                        }
                    }));

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
