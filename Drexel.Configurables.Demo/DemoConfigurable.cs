using System;
using System.Globalization;
using System.Security;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Demo
{
    public class DemoConfigurable
    {
        public const string ExpectedUsername = "ExpectedUsername";
        public const string ExpectedPasswordPlaintext = "ExpectedPassword!123";

        private const string RequirementMissingTemplate = "Missing requirement '{0}'.";

        public static SecureString ExpectedPassword = DemoConfigurable.ExpectedPasswordPlaintext.ToSecureString();
        public static Uri ExpectedWebsite = new Uri("https://www.expected.com");

        private readonly Uri website;
        private readonly string username;
        private readonly SecureString password;

        public DemoConfigurable(IBoundConfiguration configuration)
        {
            // [~9]. Try to retrieve the information we need from the IBoundConfiguration. Using the
            // information that we retrieve, set some fields on our DemoConfigurable.
            if (!configuration.TryGetOrDefault(DemoConfigurator.Website, () => null, out this.website))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        DemoConfigurable.RequirementMissingTemplate,
                        nameof(DemoConfigurator.Website)));
            }
            else if (!configuration.TryGetOrDefault(DemoConfigurator.Username, () => null, out this.username))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        DemoConfigurable.RequirementMissingTemplate,
                        nameof(DemoConfigurator.Username)));
            }
            else if (!configuration.TryGetOrDefault(DemoConfigurator.Password, () => null, out this.password))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        DemoConfigurable.RequirementMissingTemplate,
                        nameof(DemoConfigurator.Password)));
            }

            this.IsConnected = false;
        }

        public bool IsConnected { get; private set; }

        public void Connect()
        {
            // [~11]. Pretend to connect to a website.
            // We know what we expect the website, username, and password to be. If any of them are wrong,
            // then throw an exception.
            this.IsConnected = false;
            if (this.website != DemoConfigurable.ExpectedWebsite)
            {
                throw new InvalidOperationException($"Failed to connect to website '{this.website}'.");
            }
            else if (this.username != DemoConfigurable.ExpectedUsername)
            {
                throw new InvalidOperationException($"Unrecognized username '{this.username}'.");
            }
            else if (!this.password.IsEqual(DemoConfigurable.ExpectedPassword))
            {
                throw new InvalidOperationException("Wrong password.");
            }
            else
            {
                this.IsConnected = true;
            }
        }
    }
}
