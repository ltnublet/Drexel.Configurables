using System;
using System.Collections.Generic;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Demo
{
    public class DemoConfigurator : IConfigurable
    {
        static DemoConfigurator()
        {
            // [~2]. Create the requirements. For the demo, these are just examples.
            DemoConfigurator.Website = ConfigurationRequirement.Uri(
                "Website",
                "The website to use for the demo.");
            DemoConfigurator.Username = ConfigurationRequirement.String(
                "Username",
                "The username to use for the demo.");
            DemoConfigurator.Password = ConfigurationRequirement.SecureString(
                "Password",
                "The password to use for the demo.",
                dependsOn: new IConfigurationRequirement[]
                {
                    DemoConfigurator.Website,
                    DemoConfigurator.Username
                },
                additionalValidation: (x, y, z) =>
                {
                    if (!z.ContainsKey(DemoConfigurator.Website))
                    {
                        return new InvalidOperationException("Missing website.");
                    }

                    if (!z.ContainsKey(DemoConfigurator.Username))
                    {
                        return new InvalidOperationException("Missing username.");
                    }

                    Console.WriteLine("\r\nAdditional validation callback for password.");
                    Console.WriteLine(
                        "Press enter for validation to succeed. If any characters are entered "
                            + "before pressing enter, validation will fail.");
                    if (Console.ReadLine().Length != 0)
                    {
                        return new InvalidOperationException("Validation intentionally failed.");
                    }

                    return null;
                });

            DemoConfigurator.BackingRequirements = new IConfigurationRequirement[]
            {
                DemoConfigurator.Website,
                DemoConfigurator.Username,
                DemoConfigurator.Password
            };
        }

        public static IConfigurationRequirement Website;
        public static IConfigurationRequirement Username;
        public static IConfigurationRequirement Password;

        private static IConfigurationRequirement[] BackingRequirements;

        public IReadOnlyList<IConfigurationRequirement> Requirements => DemoConfigurator.BackingRequirements;

        public IBoundConfiguration Configure(IReadOnlyDictionary<IConfigurationRequirement, object> bindings)
        {
            // [~7]. For the demo, we just use the canonical implementation of the IBoundConfiguration interface,
            // which is the BoundConfiguration.

            // The BoundConfiguration object will check that our bindings are valid as part of the constructor.
            // If our bindings are invalid, the BoundConfiguration constructor will throw an exception.

            // An invalid binding would be something like:
            //     1. No username was specified
            //     2. The password was wrong
            //     3. The website was invalid (ex. we only want to allow "https", and the user specified "http")
            // The IConfigurationRequirement defines how to check if the binding is invalid, so it really depends on
            // our implementations.

            // The IConfigurable contract defines that we need to throw an ArgumentException is the supplied
            // bindings are invalid, so we wrap the AggregateException that BoundConfiguration throws.
            try
            {
                return new BoundConfiguration(this, bindings);
            }
            catch (AggregateException e)
            {
                throw new ArgumentException("The supplied bindings were invalid.", e);
            }
        }
    }
}
