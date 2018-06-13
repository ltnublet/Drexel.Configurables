using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Running the program before looking at the source code is recommended. Running the program
            // first will make understanding the code easier.

            // [~1]. Instantiate the DemoConfigurator.
            // The DemoConfigurator implements the IConfigurable interface.
            // The DemoConfigurator is what defines the requirements for the DemoConfigurable objects.
            DemoConfigurator configurator = new DemoConfigurator();

            Console.WriteLine(new string('=', 79));
            Console.WriteLine("Drexel.Configurables.Demo");
            Console.WriteLine(new string('=', 79));
            Console.WriteLine("Demo configuration properties:");

            // [~3]. Demonstrate the ability to iterate over the requirements that an IConfigurable exposes.
            foreach (IConfigurationRequirement requirement in configurator.Requirements)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"Name: '{requirement.Name}', ");
                builder.Append($"Description: '{requirement.Description}', ");
                builder.Append($"Type: '{requirement.OfType.Type}'");
                Console.WriteLine(builder.ToString());
            }

            Console.WriteLine("For connection to succeed, enter:");
            Console.WriteLine($"\tUsername: {DemoConfigurable.ExpectedUsername}");
            Console.WriteLine($"\tPassword: {DemoConfigurable.ExpectedPasswordPlaintext}");
            Console.WriteLine($"\tWebsite: {DemoConfigurable.ExpectedWebsite}");

            // [~4]. For each requirement on the DemoConfigurator, read the user's input.
            // Add the user's input to a dictionary, so that we know what was entered for each requirement.
            Dictionary<IConfigurationRequirement, object> bindings =
                new Dictionary<IConfigurationRequirement, object>();
            foreach (IConfigurationRequirement requirement in configurator.Requirements)
            {
                Console.Write($"Enter value for '{requirement.Name}': ");
                bindings.Add(requirement, Program.ReadForType(requirement.OfType.Type));
            }

            IBoundConfiguration configuration = null;
            try
            {
                // [~6]. Call the Configure method of the configurator. This will do something - the interface
                // doesn't tell us what - which will give us back an IBoundConfiguration.
                configuration = configurator.Configure(bindings);
            }
            catch (AggregateException e)
            {
                string flattenedMessages = string.Join(
                    ", ",
                    e.Flatten().InnerExceptions.Select(x => x.Message));
                Console.WriteLine($"Exception(s) while configuring bindings: {flattenedMessages}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something bad happened! {e.Message}");
            }

            if (configuration != null)
            {
                // [~8]. If the configurator was able to create an IBoundConfiguration, then we should be able
                // to instantiate the DemoConfigurable using it.
                // In a real program, it would probably make sense to hide this constructor behind a factory.
                DemoConfigurable configurable = new DemoConfigurable(configuration);

                Console.WriteLine($"Configurable is connected: '{configurable.IsConnected}'.");

                try
                {
                    // [~10]. Try to "connect" with our DemoConfigurable.
                    configurable.Connect();
                    Console.WriteLine($"Configurable is connected: '{configurable.IsConnected}'.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception while connecting: ({e.GetType()}). '{e.Message}'.");
                }
            }

            Console.WriteLine("\r\nStrike enter to exit...");
            Console.ReadLine();
        }

        private static object ReadForType(Type type)
        {
            // [~5]. This method is just for reading from the console for the demo. Notice how we make sure to
            // return the expected type for each requirement; this is because the requirements won't pass validation
            // if the type doesn't match.

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
                throw new NotImplementedException($"Demo does not implement console support for type '{type}'.");
            }
        }
    }
}
