using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Running the program before looking at the source code is recommended. Running the program
            // first will make understanding the code easier.

            // [~1]. Instantiate the DemoFactory.
            // The DemoFactory implements the IRequirementSource interface, so we know its requirements.
            // The DemoFactory implements the IConfigurator interface, so it can turn user input into a Configuration.
            DemoFactory demoFactory = new DemoFactory();

            Console.WriteLine(new string('=', 79));
            Console.WriteLine("Drexel.Configurables.Demo");
            Console.WriteLine(new string('=', 79));
            Console.WriteLine("Demo configuration properties:");

            // [~3]. Demonstrate the ability to iterate over the requirements that an IRequirementSource exposes.
            foreach (IConfigurationRequirement requirement in demoFactory.Requirements)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"Name: '{requirement.Name}', ");
                builder.Append($"Description: '{requirement.Description}', ");
                builder.Append($"Type: '{requirement.OfType.Type}'");
                Console.WriteLine(builder.ToString());
            }

            Console.WriteLine("For demo 'connection' to succeed, enter:");
            Console.WriteLine($"\tUsername: {DemoConfigurable.ExpectedUsername}");
            Console.WriteLine($"\tPassword: {DemoConfigurable.ExpectedPasswordPlaintext}");
            Console.WriteLine($"\tWebsite: {DemoConfigurable.ExpectedWebsite}");

            // [~4]. For each requirement on the DemoFactory, read the user's input.
            // Add the user's input to a dictionary, so that we know what was entered for each requirement.
            Dictionary<IConfigurationRequirement, object> bindings =
                new Dictionary<IConfigurationRequirement, object>();
            foreach (IConfigurationRequirement requirement in demoFactory.Requirements)
            {
                while (!bindings.ContainsKey(requirement))
                {
                    Console.Write($"Enter value for '{requirement.Name}': ");

                    try
                    {
                        bindings.Add(requirement, Program.ReadForType(requirement.OfType.Type));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Exception encountered processing input: {e.Message}");
                    }
                }
            }

            IConfiguration configuration = null;
            try
            {
                // [~6]. Call the Configure method of the IConfigurator. This will do something - the interface
                // doesn't tell us what - which will give us back an IConfiguration.
                configuration = demoFactory.Configure(bindings);
            }
            catch (InvalidMappingsException e)
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.AppendLine(e.Message);

                if (e.InnerException != null)
                {
                    errorMessage.AppendLine("Inner exception(s):");
                    if (e.InnerException is AggregateException aggregate)
                    {
                        foreach (Exception innerException in aggregate.InnerExceptions)
                        {
                            errorMessage.AppendLine(innerException.Message);
                        }
                    }
                    else
                    {
                        errorMessage.AppendLine(e.InnerException.Message);
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"Exception(s) while configuring bindings: {errorMessage.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something bad happened! {e.Message}");
            }

            if (configuration != null)
            {
                // [~8]. If the configurator was able to create an IConfiguration, then the DemoFactory should be able
                // to produce a DemoConfigurable for us.
                // In a real program, the DemoFactory would probably not expose the IConfigurator interface publically,
                // so that callers can't pass in illegal IConfiguration objects. For the demo, just add a check inside
                // the DemoFactory.GetInstance method.
                DemoConfigurable configurable = demoFactory.GetInstance(configuration);

                Console.WriteLine($"Configurable is connected: '{configurable.IsConnected}'.");

                try
                {
                    Console.WriteLine("Trying to connect with the demo configurable...");

                    // [~10]. Try to "connect" with our DemoConfigurable.
                    configurable.Connect();
                    Console.WriteLine($"Configurable is connected: '{configurable.IsConnected}'.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception while connecting: ({e.GetType()}). '{e.Message}'.");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press enter to exit...");
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
