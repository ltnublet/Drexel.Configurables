using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Serialization
{
    public sealed class TypeSerializerDictionary<TIntermediary>
        : IReadOnlyDictionary<RequirementType, ITypeSerializer<TIntermediary>>
    {
        private readonly IReadOnlyDictionary<RequirementType, ITypeSerializer<TIntermediary>> backingDictionary;

        internal TypeSerializerDictionary(IDictionary<RequirementType, ITypeSerializer<TIntermediary>> backingDictionary)
        {
            this.backingDictionary =
                new ReadOnlyDictionary<RequirementType, ITypeSerializer<TIntermediary>>(backingDictionary);
        }

        public ITypeSerializer<TIntermediary> this[RequirementType key] => this.backingDictionary[key];

        public IEnumerable<RequirementType> Keys => this.backingDictionary.Keys;

        public IEnumerable<ITypeSerializer<TIntermediary>> Values => this.backingDictionary.Values;

        public int Count => this.backingDictionary.Count;

        public bool ContainsKey(RequirementType key) => this.backingDictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<RequirementType, ITypeSerializer<TIntermediary>>> GetEnumerator() =>
            this.backingDictionary.GetEnumerator();

        public bool TryGetValue(RequirementType key, out ITypeSerializer<TIntermediary> value) =>
            this.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
