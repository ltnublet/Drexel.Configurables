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

        public LocalStartup(
            IExamplePlugin plugin,
            FilePath directory,
            bool includeSubFolders)
        {
            this.plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            this.directory = directory ?? throw new ArgumentNullException(nameof(directory));
            this.includeSubFolders = includeSubFolders;
        }

        public Task OnStartup(IApplicationContext context)
        {
            context.AddMenuBinding(
                new MenuBinding(
                    "List directories",
                    "Lists the directories contained by the specified directory.",
                    this.plugin,
                    (IMenuBinding self, ConsoleInstance console) =>
                    {
                        foreach (string directory in Directory.GetDirectories(
                            this.directory.Path,
                            "*",
                            this.includeSubFolders
                                ? SearchOption.AllDirectories
                                : SearchOption.TopDirectoryOnly))
                        {
                            Console.WriteLine(directory);
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
