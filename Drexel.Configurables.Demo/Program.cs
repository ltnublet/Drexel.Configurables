using Drexel.Configurables.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Drexel.Configurables.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DemoConfigurableFactory factory = new DemoConfigurableFactory();

            Console.WriteLine(new string('=', 79));
            Console.WriteLine("Drexel.Configurables.Demo");
            Console.WriteLine(new string('=', 79));

            Console.WriteLine("Demo configuration properties:");
            foreach (IConfigurationRequirement requirement in factory.Requirements)
            {
                Console.WriteLine($"Name: '{requirement.Name}', Description: '{requirement.Description}', Type: '{requirement.OfType.Type}'");
            }

            Console.WriteLine("For connection to succeed, enter:");
            Console.WriteLine($"\tUsername: {DemoConfigurable.ExpectedUsername}");
            Console.WriteLine($"\tPassword: {DemoConfigurable.ExpectedPasswordPlaintext}");
            Console.WriteLine($"\tWebsite: {DemoConfigurable.ExpectedWebsite}");

            Dictionary<IConfigurationRequirement, object> bindings =
                new Dictionary<IConfigurationRequirement, object>();
            foreach (IConfigurationRequirement requirement in factory.Requirements)
            {
                Console.Write($"Enter value for '{requirement.Name}': ");
                bindings.Add(requirement, Program.ReadForType(requirement.OfType.Type));
            }

            IBoundConfiguration configuration = factory.Configure(bindings);

            DemoConfigurable configurable = new DemoConfigurable(configuration);
            Console.WriteLine($"Configurable is connected: '{configurable.IsConnected}'.");

            try
            {
                configurable.Connect();
                Console.WriteLine($"Configurable is connected: '{configurable.IsConnected}'.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while connecting: ({e.GetType()}). '{e.Message}'.");
            }

            Console.WriteLine("\r\nStrike enter to exit...");
            Console.ReadLine();
        }

        private static object ReadForType(Type type)
        {
            if (type == typeof(string))
            {
                return Console.ReadLine();
            }
            else if (type == typeof(Uri))
            {
                return new Uri(Console.ReadLine());
            }
            else if (type == typeof(SecureString))
            {
                SecureString value = new SecureString();
                while (true)
                {
                    ConsoleKeyInfo i = Console.ReadKey(true);
                    if (i.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else if (i.Key == ConsoleKey.Backspace)
                    {
                        if (value.Length > 0)
                        {
                            value.RemoveAt(value.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    else
                    {
                        value.AppendChar(i.KeyChar);
                        Console.Write("*");
                    }
                }

                return value;
            }
            else
            {
                throw new NotImplementedException($"Frontend does not implement support for type '{type}'.");
            }
        }
    }
}
