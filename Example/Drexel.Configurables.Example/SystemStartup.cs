using System.Linq;
using System.Threading.Tasks;
using Drexel.Configurables.Example.Contracts;

namespace Drexel.Configurables.Example
{
    public class SystemStartup : IPluginStartup
    {
        private readonly SystemPlugin plugin;

        public SystemStartup(SystemPlugin plugin)
        {
            this.plugin = plugin;
        }

        public Task OnShutdown()
        {
            // Simulate the shutdown taking some time.
            return Task.Delay(2000);
        }

        public Task OnStartup(IApplicationContext context)
        {
            context.AddMenuBinding(
                new MenuBinding(
                    "List Bindings",
                    "Lists loaded bindings.",
                    this.plugin,
                    (IMenuBinding self, ConsoleInstance instance) =>
                    {
                        context.WriteBindings(instance);
                        return Task.CompletedTask;
                    }));
            context.AddMenuBinding(
                new MenuBinding(
                    "List Plugins",
                    "Lists loaded plugins.",
                    this.plugin,
                    (IMenuBinding self, ConsoleInstance instance) =>
                    {
                        instance.WriteLine("Loaded plugins:");
                        instance.WriteLine("---------------");
                        foreach (IExamplePlugin plugin in context.Bindings.Select(x => x.AssociatedPlugin).Distinct())
                        {
                            instance.WriteLine($"{plugin.Name} ({plugin.Version}) - by {plugin.Publisher}");
                        }

                        return Task.CompletedTask;
                    }));
            context.AddMenuBinding(
                new MenuBinding(
                    "Shut Down",
                    "Exits the application.",
                    this.plugin,
                    (IMenuBinding self, ConsoleInstance instance) =>
                    {
                        instance.WriteLine("Requesting shutdown...");
                        if (!context.RequestShutdown(self))
                        {
                            instance.WriteLine("Shutdown refused.");
                        }

                        return Task.CompletedTask;
                    }));

            return Task.CompletedTask;
        }
    }
}
