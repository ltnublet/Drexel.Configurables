using System;
using System.Collections.Generic;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Serialization
{
    public sealed class TypeSerializerDictionaryBuilder
    {
        private Dictionary<RequirementType, ITypeSerializer> backingDictionary;

        public TypeSerializerDictionaryBuilder()
        {
            this.backingDictionary = new Dictionary<RequirementType, ITypeSerializer>();
        }

        public void Add<T>(ClassRequirementType<T> type, IClassTypeSerializer<T> serializer)
            where T : class
        {
            if (this.backingDictionary.ContainsKey(type))
            {
                throw new ArgumentException("A serializer for the specified type has already been added.");
            }

            this.backingDictionary.Add(type, serializer);
        }

        public void Add<T>(StructRequirementType<T> type, IStructTypeSerializer<T> serializer)
            where T : struct
        {
            if (this.backingDictionary.ContainsKey(type))
            {
                throw new ArgumentException("A serializer for the specified type has already been added.");
            }

            this.backingDictionary.Add(type, serializer);
        }

        public void Add(TypeSerializerDictionary existing)
        {
            foreach (KeyValuePair<RequirementType, ITypeSerializer> pair in existing)
            {
                if (this.backingDictionary.ContainsKey(pair.Key))
                {
                    throw new ArgumentException("A serializer for the specified type has already been added.");
                }

                this.backingDictionary.Add(pair.Key, pair.Value);
            }
        }

        public TypeSerializerDictionary Build()
        {
            return new TypeSerializerDictionary(this.backingDictionary);
        }
    }
}
