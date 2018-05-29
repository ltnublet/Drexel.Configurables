using System;
using System.Collections.Generic;
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

        private Uri website;
        private string username;
        private SecureString password;

        public DemoConfigurable(IBoundConfiguration configuration)
        {
            this.website =
                (Uri)configuration.GetOrDefault(
                    DemoConfigurableFactory.Website,
                    () => null)
                as Uri
                ?? throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        DemoConfigurable.RequirementMissingTemplate,
                        nameof(DemoConfigurableFactory.Website)));
            this.username =
                (string)configuration.GetOrDefault(
                    DemoConfigurableFactory.Username,
                    () => null)
                as string
                ?? throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        DemoConfigurable.RequirementMissingTemplate,
                        nameof(DemoConfigurableFactory.Username)));
            this.password =
                (SecureString)configuration.GetOrDefault(
                    DemoConfigurableFactory.Password,
                    () => null)
                as SecureString
                ?? throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        DemoConfigurable.RequirementMissingTemplate,
                        nameof(DemoConfigurableFactory.Password)));

            this.IsConnected = false;
        }

        public bool IsConnected { get; private set; }

        public void Connect()
        {
            this.IsConnected = false;
            if (this.website != DemoConfigurable.ExpectedWebsite)
            {
                throw new InvalidOperationException($"Failed to connect to website '{this.website}'.");
            }
            else if (this.username != DemoConfigurable.ExpectedUsername)
            {
                throw new InvalidOperationException($"Unrecognized username '{this.username}'.");
            }
            else if (this.password != DemoConfigurable.ExpectedPassword)
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
