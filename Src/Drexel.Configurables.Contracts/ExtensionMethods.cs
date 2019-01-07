using System;
using System.Collections;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    internal static class ExtensionMethods
    {
        public delegate bool TryCastStruct<T>(object? value, out T result)
            where T : struct;

        public static IEnumerable<T> ToStructGenericEnumerable<T>(
            this IEnumerable nonGeneric,
            TryCastStruct<T> tryCast,
            Action<object?> onInvalid)
            where T : struct
        {
            IEnumerator enumerator = nonGeneric.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (tryCast.Invoke(enumerator.Current, out T buffer))
                {
                    yield return buffer;
                }
                else
                {
                    onInvalid.Invoke(enumerator.Current);
                }
            }
        }

        public delegate bool TryCastClass<T>(object? value, out T? result)
            where T : class;

        public static IEnumerable<T?> ToClassGenericEnumerable<T>(
            this IEnumerable nonGeneric,
            TryCastClass<T> tryCast,
            Action<object?> onInvalid)
            where T : class
        {
            IEnumerator enumerator = nonGeneric.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (tryCast.Invoke(enumerator.Current, out T? buffer))
                {
                    yield return buffer;
                }
                else
                {
                    onInvalid.Invoke(enumerator.Current);
                }
            }
        }
    }
}
