using Drexel.Configurables.Example.Contracts;

namespace Drexel.Configurables.Example
{
    internal static class IApplicationContextExtensionMethods
    {
        public static void WriteBindings(this IApplicationContext context, ConsoleInstance instance)
        {
            instance.WriteLine("Loaded bindings:");
            instance.WriteLine("----------------");
            foreach (IMenuBinding binding in context.Bindings)
            {
                instance.WriteLine($"{binding.Token} - {binding.AssociatedPlugin.Name} - {binding.Description}");
            }

            instance.WriteLine();
        }
    }
}
