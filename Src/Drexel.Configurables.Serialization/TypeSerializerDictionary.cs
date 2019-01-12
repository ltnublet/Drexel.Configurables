using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Serialization
{
    public sealed class TypeSerializerDictionary : IReadOnlyDictionary<RequirementType, ITypeSerializer>
    {
        private readonly IReadOnlyDictionary<RequirementType, ITypeSerializer> backingDictionary;

        internal TypeSerializerDictionary(IDictionary<RequirementType, ITypeSerializer> backingDictionary)
        {
            this.backingDictionary = new ReadOnlyDictionary<RequirementType, ITypeSerializer>(backingDictionary);
        }

        public ITypeSerializer this[RequirementType key] => this.backingDictionary[key];

        public IEnumerable<RequirementType> Keys => this.backingDictionary.Keys;

        public IEnumerable<ITypeSerializer> Values => this.backingDictionary.Values;

        public int Count => this.backingDictionary.Count;

        public bool ContainsKey(RequirementType key) => this.backingDictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<RequirementType, ITypeSerializer>> GetEnumerator() =>
            this.backingDictionary.GetEnumerator();

        public bool TryGetValue(RequirementType key, out ITypeSerializer value) =>
            this.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
