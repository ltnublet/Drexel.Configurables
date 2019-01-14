using System;
using System.Collections.Generic;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Serialization
{
    public sealed class TypeSerializerDictionaryBuilder<TIntermediary>
    {
        private readonly Dictionary<RequirementType, ITypeSerializer<TIntermediary>> backingDictionary;

        public TypeSerializerDictionaryBuilder()
        {
            this.backingDictionary = new Dictionary<RequirementType, ITypeSerializer<TIntermediary>>();
        }

        public TypeSerializerDictionaryBuilder<TIntermediary> Add<T>(
            ClassRequirementType<T> type,
            IClassTypeSerializer<T, TIntermediary> serializer)
            where T : class
        {
            if (this.backingDictionary.ContainsKey(type))
            {
                throw new ArgumentException("A serializer for the specified type has already been added.");
            }

            this.backingDictionary.Add(type, serializer);

            return this;
        }

        public TypeSerializerDictionaryBuilder<TIntermediary> Add<T>(
            StructRequirementType<T> type,
            IStructTypeSerializer<T, TIntermediary> serializer)
            where T : struct
        {
            if (this.backingDictionary.ContainsKey(type))
            {
                throw new ArgumentException("A serializer for the specified type has already been added.");
            }

            this.backingDictionary.Add(type, serializer);

            return this;
        }

        public TypeSerializerDictionaryBuilder<TIntermediary> Add(TypeSerializerDictionary<TIntermediary> existing)
        {
            foreach (KeyValuePair<RequirementType, ITypeSerializer<TIntermediary>> pair in existing)
            {
                if (this.backingDictionary.ContainsKey(pair.Key))
                {
                    throw new ArgumentException("A serializer for the specified type has already been added.");
                }

                this.backingDictionary.Add(pair.Key, pair.Value);
            }

            return this;
        }

        public TypeSerializerDictionary<TIntermediary> Build()
        {
            return new TypeSerializerDictionary<TIntermediary>(this.backingDictionary);
        }
    }
}
