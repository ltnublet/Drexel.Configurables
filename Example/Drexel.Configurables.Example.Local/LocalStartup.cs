using System;
using System.IO;
using System.Threading.Tasks;
using Drexel.Configurables.Example.Contracts;
using Drexel.Configurables.External;

namespace Drexel.Configurables.Example.Local
{
    public class LocalStartup : IPluginStartup
    {
        private readonly IExamplePlugin plugin;
        private readonly FilePath directory;
        private readonly bool includeSubFolders;
        private readonly string searchFilter;
        private readonly bool alwaysEmpty;

        public LocalStartup(
            IExamplePlugin plugin,
            FilePath directory,
            bool includeSubFolders = false,
            string? searchFilter = null,
            bool alwaysEmpty = false)
        {
            this.plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            this.directory = directory ?? throw new ArgumentNullException(nameof(directory));
            this.searchFilter = searchFilter ?? "*";
            this.includeSubFolders = includeSubFolders;
            this.alwaysEmpty = alwaysEmpty;
        }

        public Task OnStartup(IApplicationContext context)
        {
            context.AddMenuBinding(
                new MenuBinding(
                    "List Directories",
                    "Lists the subdirectories of the directory specified at startup.",
                    this.plugin,
                    (IMenuBinding self, ConsoleInstance console) =>
                    {
                        Console.WriteLine($"Subdirectories of directory '{this.directory.Path}' (subfolders included: {this.includeSubFolders}):");

                        if (!this.alwaysEmpty)
                        {
                            foreach (string directory in Directory.GetDirectories(
                                this.directory.Path,
                                this.searchFilter,
                                this.includeSubFolders
                                    ? SearchOption.AllDirectories
                                    : SearchOption.TopDirectoryOnly))
                            {
                                Console.WriteLine(directory);
                            }
                        }

                        return Task.CompletedTask;
                    }));

            return Task.CompletedTask;
        }

        public Task OnShutdown()
        {
            return Task.CompletedTask;
        }
    }
}
