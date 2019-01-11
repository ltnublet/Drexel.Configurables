using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a set of requirements.
    /// </summary>
    public sealed class RequirementSet : IReadOnlyDictionary<Guid, Requirement>
    {
        private readonly IReadOnlyDictionary<Guid, Requirement> backingDictionary;
        private readonly ImmutableHashSet<Requirement> backingSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementSet"/> class.
        /// </summary>
        /// <param name="requirements">
        /// The requirements to initialize this set with.
        /// </param>
        /// <exception cref="AggregateException">
        /// Occurs if multiple <see cref="RequirementSetException"/>s occur during initialization of this set.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a supplied argument is illegally <see langword="null"/>.
        /// </exception>
        /// <exception cref="DependenciesNotSatisfiedException">
        /// Thrown when the <paramref name="requirements"/> contains an <see cref="Requirement"/> which does not
        /// have all dependencies met.
        /// </exception>
        /// <exception cref="DuplicateRequirementException">
        /// Thrown when the <paramref name="requirements"/> contains a duplicate <see cref="Requirement"/>.
        /// </exception>
        /// <exception cref="RequirementTypeConflictException">
        /// Thrown when the <paramref name="requirements"/> contains conflicting <see cref="RequirementType"/>s; that
        /// is, when one <see cref="RequirementType"/> specifies an <see cref="RequirementType.Id"/> or
        /// <see cref="RequirementType.Type"/> that is incompatible with another.
        /// </exception>
        /// <exception cref="SetContainsCircularReferenceException">
        /// Thrown when the <paramref name="requirements"/> contains a cycle in either their
        /// <see cref="Requirement.DependsOn"/>s or <see cref="Requirement.ExclusiveWith"/>s.
        /// </exception>
        public RequirementSet(params Requirement[] requirements)
            : this((IReadOnlyCollection<Requirement>)requirements
                  ?? throw new ArgumentNullException(nameof(requirements)))
        {
            // Other constructor does everything.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementSet"/> class.
        /// </summary>
        /// <param name="requirements">
        /// The requirements to initialize this set with.
        /// </param>
        /// <exception cref="AggregateException">
        /// Occurs if multiple <see cref="RequirementSetException"/>s occur during initialization of this set.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a supplied argument is illegally <see langword="null"/>.
        /// </exception>
        /// <exception cref="DependenciesNotSatisfiedException">
        /// Thrown when the <paramref name="requirements"/> contains an <see cref="Requirement"/> which does not
        /// have all dependencies met.
        /// </exception>
        /// <exception cref="DuplicateRequirementException">
        /// Thrown when the <paramref name="requirements"/> contains a duplicate <see cref="Requirement"/>.
        /// </exception>
        /// <exception cref="RequirementTypeConflictException">
        /// Thrown when the <paramref name="requirements"/> contains conflicting <see cref="RequirementType"/>s; that
        /// is, when one <see cref="RequirementType"/> specifies an <see cref="RequirementType.Id"/> or
        /// <see cref="RequirementType.Type"/> that is incompatible with another.
        /// </exception>
        /// <exception cref="SetContainsCircularReferenceException">
        /// Thrown when the <paramref name="requirements"/> contains a cycle in either their
        /// <see cref="Requirement.DependsOn"/>s or <see cref="Requirement.ExclusiveWith"/>s.
        /// </exception>
        public RequirementSet(IReadOnlyCollection<Requirement> requirements)
            : this(requirements, false)
        {
            // Other constructor does everything.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementSet"/> class, optionally bypassing validation.
        /// </summary>
        /// <param name="requirements">
        /// The requirements to initialize this set with.
        /// </param>
        /// <param name="bypassValidation">
        /// <see langword="true"/> if no validation should be performed; otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="AggregateException">
        /// Occurs if multiple <see cref="RequirementSetException"/>s occur during initialization of this set.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a supplied argument is illegally <see langword="null"/>.
        /// </exception>
        /// <exception cref="DependenciesNotSatisfiedException">
        /// Thrown when the <paramref name="requirements"/> contains an <see cref="Requirement"/> which does not
        /// have all dependencies met.
        /// </exception>
        /// <exception cref="DuplicateRequirementException">
        /// Thrown when the <paramref name="requirements"/> contains a duplicate <see cref="Requirement"/>.
        /// </exception>
        /// <exception cref="RequirementTypeConflictException">
        /// Thrown when the <paramref name="requirements"/> contains conflicting <see cref="RequirementType"/>s; that
        /// is, when one <see cref="RequirementType"/> specifies an <see cref="RequirementType.Id"/> or
        /// <see cref="RequirementType.Type"/> that is incompatible with another.
        /// </exception>
        /// <exception cref="SetContainsCircularReferenceException">
        /// Thrown when the <paramref name="requirements"/> contains a cycle in either their
        /// <see cref="Requirement.DependsOn"/>s or <see cref="Requirement.ExclusiveWith"/>s.
        /// </exception>
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        internal RequirementSet(IEnumerable<Requirement> requirements, bool bypassValidation)
#pragma warning restore CS8618 // Non-nullable field is uninitialized.
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            if (bypassValidation)
            {
                this.backingDictionary = requirements.ToDictionary(x => x.Id, x => x);
                this.backingSet = this.backingDictionary.Values.ToImmutableHashSet();
                RequirementSet.TryTopologicalSort(
                    this.backingSet,
                    out IReadOnlyList<Requirement> topoBuffer,
                    (Requirement x) => x.DependsOn);
                this.TopologicallySorted = topoBuffer;
                return;
            }

            List<RequirementSetException> exceptions = new List<RequirementSetException>();
            HashSet<Requirement> distinctRequirements = new HashSet<Requirement>();
            foreach (Requirement requirement in requirements)
            {
                if (distinctRequirements.Contains(requirement))
                {
                    exceptions.Add(new DuplicateRequirementException(requirement));
                }
                else
                {
                    distinctRequirements.Add(requirement);
                }
            }

            foreach (Requirement requirement in distinctRequirements)
            {
                HashSet<Requirement> missing = new HashSet<Requirement>(requirement.DependsOn);
                missing.ExceptWith(distinctRequirements);
                if (missing.Any())
                {
                    exceptions.Add(new DependenciesNotSatisfiedException(requirement, missing));
                }
            }

            Dictionary<Guid, Requirement> types =
                new Dictionary<Guid, Requirement>(distinctRequirements.Count);
            foreach (Requirement requirement in distinctRequirements)
            {
                if (types.TryGetValue(requirement.Type.Id, out Requirement other))
                {
                    if (requirement.Type != other.Type)
                    {
                        exceptions.Add(new RequirementTypeConflictException(other, requirement));
                    }
                }
                else
                {
                    types.Add(requirement.Type.Id, requirement);
                }
            }

            if (RequirementSet.TryTopologicalSort(
                distinctRequirements,
                out IReadOnlyList<Requirement> topologicallySorted,
                (Requirement x) => x.DependsOn))
            {
                this.TopologicallySorted = topologicallySorted;
            }
            else
            {
                exceptions.Add(new SetContainsCircularReferenceException(nameof(Requirement.DependsOn)));
            }

            // TODO: Re-using topological sort to detect circular ExclusiveWith references, but throwig away result.
            if (!RequirementSet.TryTopologicalSort(
                distinctRequirements,
                out _,
                (Requirement x) => x.ExclusiveWith))
            {
                exceptions.Add(new SetContainsCircularReferenceException(nameof(Requirement.ExclusiveWith)));
            }

            if (exceptions.Count == 1)
            {
                throw exceptions[0];
            }
            else if (exceptions.Count > 1)
            {
                throw new AggregateException("Multiple validation failures occurred.", exceptions);
            }

            this.backingDictionary = distinctRequirements.ToDictionary(x => x.Id, x => x);
            this.backingSet = distinctRequirements.ToImmutableHashSet();
        }

        /// <summary>
        /// Gets the <see cref="Requirement"/> that has a <see cref="Requirement.Id"/> of <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The ID of the requirement.
        /// </param>
        /// <returns>
        /// The requirement with the same <see cref="Requirement.Id"/> as <paramref name="key"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no <see cref="Requirement"/> has a matching ID.
        /// </exception>
        public Requirement this[Guid key] => this.backingDictionary[key];

        /// <summary>
        /// Gets an empty set.
        /// </summary>
        public static RequirementSet Empty { get; } = new RequirementSet(Array.Empty<Requirement>());

        /// <summary>
        /// Gets the number of <see cref="Requirement"/>s in the set.
        /// </summary>
        public int Count => this.backingDictionary.Count;

        /// <summary>
        /// Gets the <see cref="Guid"/>s corresponding to the IDs of the <see cref="Requirement"/>s contained by this
        /// set.
        /// </summary>
        public IEnumerable<Guid> Keys => this.backingDictionary.Keys;

        /// <summary>
        /// Gets the <see cref="Requirement"/>s contained by this set.
        /// </summary>
        public IEnumerable<Requirement> Values => this.backingDictionary.Values;

        /// <summary>
        /// Gets the requirements contained by this set in topological order (sorted by their dependencies).
        /// </summary>
        public IReadOnlyList<Requirement> TopologicallySorted { get; }

        /// <summary>
        /// Returns a value indicating whether this set contains a <see cref="Requirement"/> with the an ID equal to
        /// the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this set contains a <see cref="Requirement"/> with a matching ID; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(Guid key)
        {
            return this.backingDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Returns the contents of this set, except the contents of the specified set. The result is the values
        /// not present in the other set.
        /// </summary>
        /// <param name="other">
        /// The othe rset.
        /// </param>
        /// <returns>
        /// The values not present in the other set.
        /// </returns>
        public IReadOnlyCollection<Requirement> Except(IEnumerable<Requirement> other)
        {
            return this.backingSet.Except(other);
        }

        /// <summary>
        /// Returns a value indicating whether this set is a superset of the the specified set.
        /// </summary>
        /// <param name="other">
        /// The other set.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this set is a superset of the specified set; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="other"/> is <see langword="null"/>.
        /// </exception>
        public bool IsSupersetOf(IEnumerable<Requirement> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return other.All(x => this.Values.Contains(x));
        }

        /// <summary>
        /// Returns the intersection between this set, and the specified set. The intersection consists of
        /// <see cref="Requirement"/>s contained by both sets.
        /// </summary>
        /// <param name="other">
        /// The other set.
        /// </param>
        /// <returns>
        /// The intersection of the two sets.
        /// </returns>
        public IReadOnlyCollection<Requirement> Intersect(IEnumerable<Requirement> other)
        {
            return this.backingSet.Intersect(other);
        }

        /// <summary>
        /// Gets the <see cref="Requirement"/> that has a <see cref="Requirement.Id"/> of <paramref name="key"/>, if
        /// one exists.
        /// </summary>
        /// <param name="key">
        /// The ID of the requirement.
        /// </param>
        /// <param name="value">
        /// When this method returns, the <see cref="Requirement"/> with the specified key, if the key is found;
        /// otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// The requirement with the same <see cref="Requirement.Id"/> as <paramref name="key"/>.
        /// </returns>
        public bool TryGetValue(Guid key, out Requirement value) => this.backingDictionary.TryGetValue(key, out value);

        /// <summary>
        /// Returns an enumerator that iterates through the set.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates through the set.
        /// </returns>
        public IEnumerator<KeyValuePair<Guid, Requirement>> GetEnumerator() => this.backingDictionary.GetEnumerator();

        /// <summary>
        /// Tries to topologically sort the specified requirements. It is assumed that they have otherwise passed
        /// validation.
        /// </summary>
        /// <param name="requirements">
        /// The requirements to sort.
        /// </param>
        /// <param name="result">
        /// The results.
        /// </param>
        /// <param name="accessor">
        /// The manner in which dependencies are retrieved.
        /// </param>
        /// <returns>
        /// <see langword="false"/> if the specified requirements have a circular dependency; otherwise,
        /// <see langword="true"/>.
        /// </returns>
        private static bool TryTopologicalSort(
            IReadOnlyCollection<Requirement> requirements,
            out IReadOnlyList<Requirement> result,
            Func<Requirement, IReadOnlyCollection<Requirement>> accessor)
        {
            HashSet<Tuple<Requirement, Requirement>> dependencies =
                new HashSet<Tuple<Requirement, Requirement>>(requirements
                    .SelectMany(
                        requirement =>
                        accessor
                            .Invoke(requirement)
                            .Select(dependency => new Tuple<Requirement, Requirement>(dependency, requirement))));

            List<Requirement> resultBuffer = new List<Requirement>();
            HashSet<Requirement> buffer = new HashSet<Requirement>(requirements.Where(x => !accessor.Invoke(x).Any()));

            while (buffer.Count != 0)
            {
                Requirement next = buffer.First();
                buffer.Remove(next);

                resultBuffer.Add(next);

                foreach (Tuple<Requirement, Requirement> dependency in
                    dependencies.Where(x => x.Item1.Equals(next)).ToArray())
                {
                    Requirement dependent = dependency.Item2;
                    dependencies.Remove(dependency);

                    if (!dependencies.Any(x => x.Item2.Equals(dependent)))
                    {
                        buffer.Add(dependent);
                    }
                }
            }

            result = resultBuffer;
            if (dependencies.Any())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the set.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates through the set.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
