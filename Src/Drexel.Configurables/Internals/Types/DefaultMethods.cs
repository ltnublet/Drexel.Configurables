using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.Configurables.Internals.Types
{
    /// <summary>
    /// Static default <see cref="Contracts.RequirementType"/> helper methods. Intended to be used when creating new
    /// <see cref="Contracts.RequirementType"/>s with an inner <see cref="Contracts.RequirementType.Type"/> for which
    /// no built-in type exists.
    /// </summary>
    public static class DefaultMethods
    {
        /// <summary>
        /// Tries to cast the specified <paramref name="value"/> to a <see langword="struct"/> of type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast the value to.
        /// </typeparam>
        /// <param name="value">
        /// The value to cast.
        /// </param>
        /// <param name="result">
        /// The result of the cast.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the cast was successful; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryCastStructValue<T>(object? value, out T result)
            where T : struct
        {
            if (value == null)
            {
                result = default;
                return false;
            }
            else if (value is T asT)
            {
                result = asT;
                return true;
            }
            else
            {
                try
                {
                    result = (T)value;
                    return true;
                }
                catch
                {
                    result = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// Tries to cast the specified <paramref name="value"/> to a <see langword="class"/> of type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast the value to.
        /// </typeparam>
        /// <param name="value">
        /// The value to cast.
        /// </param>
        /// <param name="result">
        /// The result of the cast.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the cast was successful; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryCastClassValue<T>(object? value, out T? result)
            where T : class
        {
            if (value == null)
            {
                result = null;
                return true;
            }
            else if (value is T asT)
            {
                result = asT;
                return true;
            }
            else
            {
                try
                {
                    result = (T?)value;
                    return true;
                }
                catch
                {
                    result = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// Tries to cast the specified <paramref name="value"/> to a collection of <see langword="struct"/>s of type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast the value to.
        /// </typeparam>
        /// <param name="value">
        /// The value to cast.
        /// </param>
        /// <param name="result">
        /// The result of the cast.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the cast was successful; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryCastStructCollection<T>(object? value, out IEnumerable<T>? result)
            where T : struct
        {
            if (value == null)
            {
                result = null;
                return true;
            }
            else if (value is IEnumerable<T> asGenericEnumerable)
            {
                result = asGenericEnumerable;
                return true;
            }
            else
            {
                try
                {
                    result = (IEnumerable<T>?)value;
                    return true;
                }
                catch
                {
                    try
                    {
                        if (value is IEnumerable asEnumerable)
                        {
                            result = asEnumerable.Cast<T>().ToArray();
                            return true;
                        }
                    }
                    catch
                    {
                        result = default;
                        return false;
                    }

                    result = default;
                    return false;
                }
            }
        }


        /// <summary>
        /// Tries to cast the specified <paramref name="value"/> to a collection of <see langword="class"/>es of type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast the value to.
        /// </typeparam>
        /// <param name="value">
        /// The value to cast.
        /// </param>
        /// <param name="result">
        /// The result of the cast.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the cast was successful; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryCastClassCollection<T>(object? value, out IEnumerable<T?>? result)
            where T : class
        {
            if (value == null)
            {
                result = null;
                return true;
            }
            else if (value is IEnumerable<T?> asGenericEnumerable)
            {
                result = asGenericEnumerable;
                return true;
            }
            else
            {
                try
                {
                    result = (IEnumerable<T?>?)value;
                    return true;
                }
                catch
                {
                    try
                    {
                        if (value is IEnumerable asEnumerable)
                        {
                            result = asEnumerable.Cast<T?>().ToArray();
                            return true;
                        }
                    }
                    catch
                    {
                        result = default;
                        return false;
                    }

                    result = default;
                    return false;
                }
            }
        }
    }
}
