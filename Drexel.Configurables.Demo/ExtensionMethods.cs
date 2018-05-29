using System.Security;

namespace Drexel.Configurables.Demo
{
    public static class ExtensionMethods
    {
        public static SecureString ToSecureString(this string source)
        {
            char[] charArray = source.ToCharArray();
            unsafe
            {
                fixed (char* chars = charArray)
                {
                    return new SecureString(chars, charArray.Length);
                }
            }
        }
    }
}
