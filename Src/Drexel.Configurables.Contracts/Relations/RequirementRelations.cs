using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyValueType = System.Collections.Generic.IReadOnlyCollection<
    System.Collections.Generic.KeyValuePair<
        Drexel.Configurables.Contracts.Requirement,
        Drexel.Configurables.Contracts.Relations.RequirementRelation>>;
using MyType = System.Collections.Generic.IReadOnlyDictionary<
    Drexel.Configurables.Contracts.Requirement,
    System.Collections.Generic.IReadOnlyCollection<
        System.Collections.Generic.KeyValuePair<
            Drexel.Configurables.Contracts.Requirement,
            Drexel.Configurables.Contracts.Relations.RequirementRelation>>>;
using System.Linq;

namespace Drexel.Configurables.Contracts.Relations
{
    public sealed class RequirementRelations : MyType
    {
        private readonly ReadOnlyDictionary<Requirement, ReadOnlyDictionary<Requirement, RequirementRelation>>
            mappings;

        internal RequirementRelations(
            Dictionary<Requirement, Dictionary<Requirement, RequirementRelation>> primaryToSecondary,
            List<RequirementTree> trees)
        {
            Dictionary<Requirement, ReadOnlyDictionary<Requirement, RequirementRelation>> primaryToSecondaryBuffer =
                new Dictionary<Requirement, ReadOnlyDictionary<Requirement, RequirementRelation>>(
                    primaryToSecondary.Count);

            foreach (KeyValuePair<Requirement, Dictionary<Requirement, RequirementRelation>> buffer in
                primaryToSecondary)
            {
                primaryToSecondaryBuffer.Add(
                    buffer.Key,
                    new ReadOnlyDictionary<Requirement, RequirementRelation>(buffer.Value));
            }

            this.mappings =
                new ReadOnlyDictionary<Requirement, ReadOnlyDictionary<Requirement, RequirementRelation>>(
                    primaryToSecondaryBuffer);

            this.Forest = new RequirementForest(trees);
        }

        public MyValueType this[Requirement key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (!this.mappings.TryGetValue(
                    key,
                    out ReadOnlyDictionary<Requirement, RequirementRelation> value))
                {
                    throw new KeyNotFoundException();
                }

                return value;
            }
        }

        public IReadOnlyCollection<Requirement> Keys => this.mappings.Keys;

        public IReadOnlyCollection<MyValueType> Values => this.mappings.Values;

        public int Count => this.mappings.Count;

        public RequirementForest Forest { get; }

        public bool ContainsKey(Requirement key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return this.mappings.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<Requirement, MyValueType>> GetEnumerator()
        {
            return this
                .mappings
                .Select(x => new KeyValuePair<Requirement, MyValueType>(x.Key, x.Value))
                .GetEnumerator();
        }

        public RequirementRelation GetRelation(Requirement primary, Requirement secondary)
        {
            if (this.mappings.TryGetValue(
                primary,
                out ReadOnlyDictionary<Requirement, RequirementRelation> buffer))
            {
                if (buffer.TryGetValue(secondary, out RequirementRelation value))
                {
                    return value;
                }
            }

            return RequirementRelation.None;
        }

        public bool TryGetValue(Requirement key, out MyValueType value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            bool contained = this.mappings.TryGetValue(
                key,
                out ReadOnlyDictionary<Requirement, RequirementRelation> buffer);

            value = buffer;
            return contained;
        }

        IEnumerable<Requirement> MyType.Keys =>
            this.Keys;

        IEnumerable<MyValueType> MyType.Values =>
            this.Values;

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
