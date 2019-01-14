using System.Security;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.Tests
{
    public static class ExtensionMethods
    {
        public static SecureString ToSecureString(this string @string)
        {
            SecureString secureString = new SecureString();

            foreach (char @char in @string)
            {
                secureString.AppendChar(@char);
            }

            return secureString;
        }
    }
}
