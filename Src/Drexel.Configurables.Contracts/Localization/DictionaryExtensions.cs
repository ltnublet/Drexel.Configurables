using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Localization
{
    internal static class DictionaryExtensions
    {
        public static U GetOrDefault<T, U>(this IReadOnlyDictionary<T, U> dictionary, T key, U defaultValue) =>
            dictionary.GetOrDefault(key, () => defaultValue);

        public static U GetOrDefault<T, U>(this IReadOnlyDictionary<T, U> dictionary, T key, Func<U> defaultValueFactory)
        {
            U buffer;
            if (!dictionary.TryGetValue(key, out buffer))
            {
                buffer = defaultValueFactory.Invoke();
            }

            return buffer;
        }
    }
}
