using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Demo
{
    public class DemoFactory : IRequirementSource, IConfigurator
    {
        private const string RequirementMissingTemplate = "Missing requirement '{0}'.";

        static DemoFactory()
        {
            // [~2]. Create the requirements. For the demo, these are just examples.
            DemoFactory.Website = ConfigurationRequirement.Uri(
                "Website",
                "The website to use for the demo.");
            DemoFactory.Username = ConfigurationRequirement.String(
                "Username",
                "The username to use for the demo.");
            DemoFactory.Password = ConfigurationRequirement.SecureString(
                "Password",
                "The password to use for the demo.",
                dependsOn: new IConfigurationRequirement[]
                {
                    DemoFactory.Website,
                    DemoFactory.Username
                },
                additionalValidation: (x, y, z) =>
                {
                    if (!z.TryGetOrDefault(DemoFactory.Website, () => null, out Uri website))
                    {
                        return new InvalidOperationException("Missing website.");
                    }

                    if (!z.TryGetOrDefault(DemoFactory.Username, () => null, out string username))
                    {
                        return new InvalidOperationException("Missing username.");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Additional validation callback for password.");
                    Console.WriteLine("Press enter to continue. Non-empty inputs will intentionally fail validation.");
                    string input = Console.ReadLine();
                    if (input.Length != 0)
                    {
                        return new InvalidOperationException(input);
                    }

                    return null;
                });

            DemoFactory.BackingRequirements = new IConfigurationRequirement[]
            {
                DemoFactory.Website,
                DemoFactory.Username,
                DemoFactory.Password
            };
        }

        public static IConfigurationRequirement Website;
        public static IConfigurationRequirement Username;
        public static IConfigurationRequirement Password;

        private static IConfigurationRequirement[] BackingRequirements;

        public IReadOnlyList<IConfigurationRequirement> Requirements => DemoFactory.BackingRequirements;

        public IConfiguration Configure(IReadOnlyDictionary<IConfigurationRequirement, object> mappings)
        {
            // [~7]. For the demo, we just use the canonical implementation of the IConfiguration interface,
            // which is the Drexel.Configurables.Configuration object.

            // The Configuration object will check that our mappings are valid as part of the constructor.
            // If our mappings are invalid, the Configuration constructor will throw an exception.

            // An invalid mapping would be something like:
            //     1. No username was specified
            //     2. The password was wrong
            //     3. The website was invalid (ex. we only want to allow "https", and the user specified "http")
            // The IConfigurationRequirement defines how to check if the mapping is invalid, so it really depends on
            // our implementations.

            // The IConfigurator contract defines that we need to throw an InvalidMappingsException if the supplied
            // mappings are invalid, so we wrap the AggregateException that Configuration throws.
            try
            {
                return new Configuration(this, mappings, this);
            }
            catch (AggregateException e)
            {
                throw new InvalidMappingsException("The supplied mappings were invalid.", e);
            }
        }

        public DemoConfigurable GetInstance(IConfiguration configuration)
        {
            if (configuration.Configurator != this)
            {
                throw new ArgumentException("Supplied configuration was created by a different IConfigurator.");
            }

            // [~9]. Try to retrieve the information we need from the IConfiguration.
            // Then, using the information, instantiate a new DemoConfigurable.
            Uri website;
            string username;
            SecureString password;

            if (!configuration.TryGetOrDefault(DemoFactory.Website, () => null, out website))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        DemoFactory.RequirementMissingTemplate,
                        nameof(DemoFactory.Website)));
            }
            else if (!configuration.TryGetOrDefault(DemoFactory.Username, () => null, out username))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        DemoFactory.RequirementMissingTemplate,
                        nameof(DemoFactory.Username)));
            }
            else if (!configuration.TryGetOrDefault(DemoFactory.Password, () => null, out password))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        DemoFactory.RequirementMissingTemplate,
                        nameof(DemoFactory.Password)));
            }

            return new DemoConfigurable(website, username, password);
        }
    }
}
