using System;
using System.Text;

namespace Drexel.Configurables.External.Internals
{
    internal static class StringExtensionMethods
    {
        public static string Escape(this string toEscape)
        {
            if (toEscape == null)
            {
                throw new ArgumentNullException(nameof(toEscape));
            }

            // Preallocate memory for the worst case, which is that every character in the string must be escaped.
            StringBuilder builder = new StringBuilder(toEscape.Length * 2);
            foreach (char @char in toEscape)
            {
                switch (@char)
                {
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append(@"\");
                        break;
                    default:
                        builder.Append(@char);
                        break;
                }
            }

            return builder.ToString();
        }
    }
}
