using System;
using System.Text;

namespace Drexel.Configurables.External.Internals
{
    /// <summary>
    /// Internal <see langword="string"/> extension methods.
    /// </summary>
    internal static class StringExtensionMethods
    {
        /// <summary>
        /// Escapes the specified <see langword="string"/> such that it is safe for JSON.
        /// </summary>
        /// <param name="toEscape">
        /// The <see langword="string"/> to escape.
        /// </param>
        /// <returns>
        /// An escaped version of the supplied <see langword="string"/>.
        /// </returns>
        public static string JsonEscape(this string toEscape)
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
