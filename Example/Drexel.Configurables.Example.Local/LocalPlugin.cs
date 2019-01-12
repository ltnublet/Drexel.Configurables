using System;
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
            LocalPlugin.DirectoryRequirement = StandardRequirements.CreateFilePath(
                Guid.NewGuid(),
                restrictedToSet: new ClassSetRestrictionInfo<FilePath>[]
                    {
                        new ClassSetRestrictionInfo<FilePath>(new FilePath(@"C:\")),
                        new ClassSetRestrictionInfo<FilePath>(new FilePath(@"D:\")),
                        new ClassSetRestrictionInfo<FilePath>(new FilePath(@"E:\"))
                    });
            LocalPlugin.IncludeSubfoldersRequirement = StandardRequirements.CreateBoolean(
                Guid.NewGuid(),
                isOptional: true);
            LocalPlugin.SearchFilterRequirement = StandardRequirements.CreateString(
                Guid.NewGuid(),
                isOptional: true,
                relations: new RequirementRelationsBuilder()
                    .AddExclusiveWith(LocalPlugin.IncludeSubfoldersRequirement)
                    .Build());
            LocalPlugin.EmptyEvenIfHasDirectoriesRequirement = StandardRequirements.CreateBoolean(
                Guid.NewGuid(),
                isOptional: true,
                relations: new RequirementRelationsBuilder()
                    .AddDependsOn(LocalPlugin.DirectoryRequirement)
                    .Build());

            LocalPlugin.RequirementSet = new RequirementSet(
                LocalPlugin.DirectoryRequirement,
                LocalPlugin.IncludeSubfoldersRequirement,
                LocalPlugin.SearchFilterRequirement,
                LocalPlugin.EmptyEvenIfHasDirectoriesRequirement);
        }

        private static readonly ClassRequirement<FilePath> DirectoryRequirement;
        private static readonly StructRequirement<bool> IncludeSubfoldersRequirement;
        private static readonly ClassRequirement<string> SearchFilterRequirement;
        private static readonly StructRequirement<bool> EmptyEvenIfHasDirectoriesRequirement;
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
            FilePath? directory = configuration.Get(LocalPlugin.DirectoryRequirement);
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(LocalPlugin.DirectoryRequirement));
            }

            bool includeSubfolders = configuration.GetOrDefault(LocalPlugin.IncludeSubfoldersRequirement, () => false);
            string? searchFilter = configuration.GetOrDefault(LocalPlugin.SearchFilterRequirement, () => null);

            bool emptyEvenIfHasDirectories =
                configuration.GetOrDefault(LocalPlugin.EmptyEvenIfHasDirectoriesRequirement, () => false);

            return new LocalStartup(
                this,
                directory,
                includeSubfolders,
                searchFilter,
                emptyEvenIfHasDirectories);
        }
    }
}
