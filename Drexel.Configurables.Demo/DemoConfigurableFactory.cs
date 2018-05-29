using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Demo
{
    public class DemoConfigurableFactory : IConfigurable
    {
        static DemoConfigurableFactory()
        {
            DemoConfigurableFactory.Website = ConfigurationRequirement.Uri(
                "Website",
                "The website to use for the demo.");
            DemoConfigurableFactory.Username = ConfigurationRequirement.String(
                "Username",
                "The username to use for the demo.");
            DemoConfigurableFactory.Password = ConfigurationRequirement.SecureString(
                "Password",
                "The password to use for the demo.",
                dependsOn: new IConfigurationRequirement[]
                {
                    DemoConfigurableFactory.Website,
                    DemoConfigurableFactory.Username
                },
                additionalValidation: (x, y, z) =>
                {
                    if (!z.ContainsKey(DemoConfigurableFactory.Username))
                    {
                        return new InvalidOperationException("Dependency validation failure!!");
                    }

                    return null;
                });

            DemoConfigurableFactory.BackingRequirements = new IConfigurationRequirement[]
            {
                DemoConfigurableFactory.Website,
                DemoConfigurableFactory.Username,
                DemoConfigurableFactory.Password
            };
        }

        public static IConfigurationRequirement Website;
        public static IConfigurationRequirement Username;
        public static IConfigurationRequirement Password;

        private static IConfigurationRequirement[] BackingRequirements;

        public IReadOnlyList<IConfigurationRequirement> Requirements => DemoConfigurableFactory.BackingRequirements;

        public IBoundConfiguration Configure(IReadOnlyDictionary<IConfigurationRequirement, object> bindings)
        {
            return new BoundConfiguration(this, bindings);
        }
    }
}
