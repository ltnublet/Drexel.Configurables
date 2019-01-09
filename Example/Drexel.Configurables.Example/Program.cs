using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Example.Contracts;
using Drexel.Configurables.External;

namespace Drexel.Configurables.Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Program program = new Program();
            await program.Run();
        }

        public async Task Run()
        {
            // Load the plugins using Microsoft Extensibility Framework.
            PluginLoader loader = new PluginLoader();
            loader.LoadPlugins();

            // Collect the configuration requirements for the plugins, and use it to create instances of them.
            List<IPluginStartup> startups = new List<IPluginStartup>();
            foreach (IExamplePlugin plugin in loader.LoadedPlugins)
            {
                Configuration pluginConfiguration = await Program.CreateConfigurationFromIConfigurable(plugin);
                startups.Add(plugin.CreateStartup(pluginConfiguration));
            }

            // Create our application context.
            ApplicationContext context = new ApplicationContext();

            {
                // Start the plugins.
                List<Task> pluginStartups = new List<Task>();
                foreach (IPluginStartup startup in startups)
                {
                    pluginStartups.Add(startup.OnStartup(context));
                }

                await Task.WhenAll(pluginStartups);
            }

            // Set ourselves up to wait for the shutdown event.
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            context.ShutdownRequested += (obj, e) => cancellationTokenSource.Cancel();

            // Let the user see what commands are available to them.
            Program.WriteAvailableCommandsToConsole(context);

            // Start listening for commands from the user.
            ConsoleInstance console = new ConsoleInstance();
            Console.CancelKeyPress +=
                (obj, e) =>
                {
                    // Indicate to the console that the shutdown has been handled.
                    e.Cancel = true;

                    // Begin graceful shutdown.
                    context.ShutDown(null);
                };

            context.ShutdownRequested +=
                (obj, e) =>
                {
                    if (e == null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("User has requested a shutdown.");
                    }
                    else
                    {
                        Console.WriteLine($"Plugin '{e.Name}' has requested a shutdown.");
                    }
                };

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                Console.Write("Enter case-sensitive token: ");
                string token = Console.ReadLine();

                // Happens on Ctrl + C.
                if (token == null)
                {
                    Console.WriteLine("Ctrl+C received.");

                    // The while loop is so tight that we can sometimes read the token before it gets canceled, so
                    // just break to ensure we exit.
                    break;
                }

                if (context.Bindings.TryGetValue(token, out IMenuBinding binding))
                {
                    await binding.Callback(binding, console);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No matching case-sensitive token exists.");
                    Program.WriteAvailableCommandsToConsole(context);
                }
            }

            // Cancellation was requested; wait for the plugins to shut down, and then let the user know we're done.
            await Task.WhenAll(startups.Select(x => x.OnShutdown()));
            Console.WriteLine();
            Console.WriteLine("The application has shut down. Press enter to exit.");
            Console.ReadLine();
        }

        private static async Task<Configuration> CreateConfigurationFromIConfigurable(IConfigurable configurable)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            foreach (Requirement requirement in configurable.Requirements.Values)
            {
                // TODO: Make input libraries
                object? value = null;
                Type type = requirement.Type.Type;
                if (type == typeof(FilePath))
                {
                    Console.Write("Enter file path (press enter to skip): ");
                    string input = Console.ReadLine();
                    if (input == string.Empty)
                    {
                        continue;
                    }

                    value = new FilePath(input, PathInteractor.Instance);
                }
                else if (type == typeof(bool))
                {
                    Console.Write("Enter bool (press enter to skip): ");
                    string input = Console.ReadLine();
                    if (input == string.Empty)
                    {
                        continue;
                    }

                    value = bool.Parse(input);
                }
                else if (type == typeof(string))
                {
                    Console.Write("Enter string (press enter to skip): ");
                    string input = Console.ReadLine();
                    if (input == string.Empty)
                    {
                        continue;
                    }

                    value = input;
                }
                else
                {
                    throw new Exception("Unknown requirement type.");
                }

                builder.Add(requirement, value);
            }

            return await builder.Build(configurable.Requirements);
        }

        private static void WriteAvailableCommandsToConsole(ApplicationContext context)
        {
            context.WriteBindings(new ConsoleInstance());
            Console.WriteLine("----------------");
            Console.WriteLine();
        }

        public class PluginLoader
        {
            public PluginLoader()
            {
                this.LoadedPlugins = Array.Empty<IExamplePlugin>();
            }

            [ImportMany]
            public IEnumerable<IExamplePlugin> LoadedPlugins { get; set; }

            public void LoadPlugins()
            {
                string assemblyLocation = AppDomain.CurrentDomain.BaseDirectory;
                IReadOnlyList<Assembly> potentialPluginAssemblies = PluginLoader
                    .Transform(Directory.GetFiles(assemblyLocation))
                    .ToList();
                ContainerConfiguration hostingProvider = new ContainerConfiguration()
                    .WithAssemblies(potentialPluginAssemblies);

                using (CompositionHost container = hostingProvider.CreateContainer())
                {
                    this.LoadedPlugins = container.GetExports<IExamplePlugin>();
                }
            }

            private static IEnumerable<Assembly> Transform(IEnumerable<string> paths)
            {
                foreach (string path in paths)
                {
                    Assembly? assembly = null;

                    try
                    {
                        assembly = Assembly.LoadFrom(path);
                    }
                    catch (Exception)
                    {
                        // Do nothing.
                    }

                    if (assembly != null)
                    {
                        yield return assembly;
                    }
                }
            }
        }
    }
}
