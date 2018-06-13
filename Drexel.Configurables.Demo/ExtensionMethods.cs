using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Drexel.Configurables.Demo
{
    internal static class ExtensionMethods
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

        public static bool IsEqual(this SecureString left, SecureString right)
        {
            IntPtr binaryStringLeft = IntPtr.Zero;
            IntPtr binaryStringRight = IntPtr.Zero;

            try
            {
                binaryStringLeft = Marshal.SecureStringToBSTR(left);
                binaryStringRight = Marshal.SecureStringToBSTR(right);

                int leftLength = Marshal.ReadInt32(binaryStringLeft, -4);
                int rightLength = Marshal.ReadInt32(binaryStringRight, -4);

                if (leftLength != rightLength)
                {
                    return false;
                }

                for (int index = 0; index < leftLength; index++)
                {
                    if (Marshal.ReadByte(binaryStringLeft, index)
                        != Marshal.ReadByte(binaryStringRight, index))
                    {
                        return false;
                    }
                }

                return true;
            }
            finally
            {
                if (binaryStringRight != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(binaryStringRight);
                }

                if (binaryStringLeft != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(binaryStringLeft);
                }
            }
        }
    }
}
