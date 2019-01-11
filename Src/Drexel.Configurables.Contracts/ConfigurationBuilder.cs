using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Contracts
{
    public sealed class ConfigurationBuilder
    {
        private readonly Dictionary<Requirement, IEnumerable> collectionMappings;
        private readonly Dictionary<Requirement, dynamic?> singleMappings;

        public ConfigurationBuilder()
        {
            this.collectionMappings = new Dictionary<Requirement, IEnumerable>();
            this.singleMappings = new Dictionary<Requirement, dynamic?>();
        }

        public void Add(Requirement requirement, object? value)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            else if (requirement.CollectionInfo.HasValue)
            {
                throw new ArgumentException(
                    "Collection requirements must supply entire collection.",
                    nameof(requirement));
            }
            else if (requirement.ExclusiveWith.Any(
                x =>
                this.singleMappings.ContainsKey(x) || this.collectionMappings.ContainsKey(x)))
            {
                throw new ArgumentException("Specified requirement is exclusive with an already added requirement.");
            }
            else if (!requirement.Type.TryCast(value, out object? result))
            {
                throw new ArgumentException("Could not cast supplied value to requirement type.", nameof(value));
            }
            else
            {
                this.singleMappings.Add(requirement, result);
            }
        }

        public void Add(Requirement requirement, IEnumerable value)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            else if (!requirement.CollectionInfo.HasValue)
            {
                throw new ArgumentException(
                    "Non-collection requirements must supply an individual value.",
                    nameof(requirement));
            }
            else if (requirement.ExclusiveWith.Any(
                x =>
                this.singleMappings.ContainsKey(x) || this.collectionMappings.ContainsKey(x)))
            {
                throw new ArgumentException("Specified requirement is exclusive with an already added requirement.");
            }
            else if (!requirement.Type.TryCast(value, out IEnumerable? result))
            {
                throw new ArgumentException(
                    "Could not cast supplied value to a collection of requirement type.",
                    nameof(value));
            }
            else if (result == null)
            {
                throw new InvalidOperationException("Cast resulted in a null enumerable.");
            }
            else
            {
                this.collectionMappings.Add(requirement, result);
            }
        }

        public void Add<T>(ClassRequirement<T> requirement, IEnumerable<T?> value)
            where T : class
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            else if (!requirement.CollectionInfo.HasValue)
            {
                throw new ArgumentException("Non-collection requirements must supply an individual value.");
            }
            else if (requirement.ExclusiveWith.Any(
                x =>
                this.singleMappings.ContainsKey(x) || this.collectionMappings.ContainsKey(x)))
            {
                throw new ArgumentException("Specified requirement is exclusive with an already added requirement.");
            }

            this.collectionMappings.Add(requirement, value);
        }

        public void Add<T>(ClassRequirement<T> requirement, T? value)
            where T : class
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            else if (requirement.CollectionInfo.HasValue)
            {
                throw new ArgumentException("Collection requirements must supply entire collection.");
            }
            else if (requirement.ExclusiveWith.Any(
                x =>
                this.singleMappings.ContainsKey(x) || this.collectionMappings.ContainsKey(x)))
            {
                throw new ArgumentException("Specified requirement is exclusive with an already added requirement.");
            }

            this.singleMappings.Add(requirement, value);
        }

        public void Add<T>(StructRequirement<T> requirement, IEnumerable<T> value)
            where T : struct
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            else if (!requirement.CollectionInfo.HasValue)
            {
                throw new ArgumentException("Non-collection requirements must supply an individual value.");
            }
            else if (requirement.ExclusiveWith.Any(
                x =>
                this.singleMappings.ContainsKey(x) || this.collectionMappings.ContainsKey(x)))
            {
                throw new ArgumentException("Specified requirement is exclusive with an already added requirement.");
            }

            this.collectionMappings.Add(requirement, value);
        }

        public void Add<T>(StructRequirement<T> requirement, T value)
            where T : struct
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            else if (requirement.CollectionInfo.HasValue)
            {
                throw new ArgumentException("Collection requirements must supply entire collection.");
            }
            else if (requirement.ExclusiveWith.Any(
                x =>
                this.singleMappings.ContainsKey(x) || this.collectionMappings.ContainsKey(x)))
            {
                throw new ArgumentException("Specified requirement is exclusive with an already added requirement.");
            }

            this.singleMappings.Add(requirement, value);
        }

        public async Task<Configuration> Build(RequirementSet requirements)
        {
            ImmutableDictionary<Requirement, dynamic?> completedBindings =
                new Dictionary<Requirement, dynamic?>().ToImmutableDictionary();
            List<Exception> exceptions = new List<Exception>();
            foreach (Requirement requirement in requirements.TopologicallySorted)
            {
                Configuration completed = new Configuration(
                    new RequirementSet(completedBindings.Keys, true),
                    completedBindings);
                if (this.singleMappings.TryGetValue(requirement, out dynamic? singleBuffer))
                {
                    try
                    {
                        await requirement.Validate(singleBuffer, completed).ConfigureAwait(false);
                        completedBindings = completedBindings.Add(requirement, singleBuffer);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }
                else if (this.collectionMappings.TryGetValue(requirement, out IEnumerable collectionBuffer))
                {
                    try
                    {
                        requirement.SetValidator.Validate(collectionBuffer);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }

                    foreach (object? value in collectionBuffer)
                    {
                        try
                        {
                            await requirement.Validate(value, completed).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            exceptions.Add(e);
                        }
                    }

                    completedBindings = completedBindings.Add(requirement, collectionBuffer);
                }
                else if (!requirement.IsOptional)
                {
                    exceptions.Add(new ArgumentException("Missing requirement."));
                    continue;
                }
            }

            if (exceptions.Count == 1)
            {
                throw exceptions[0];
            }
            else if (exceptions.Count > 1)
            {
                throw new AggregateException(
                    "Multiple exceptions occurred.",
                    exceptions);
            }
            else
            {
                return new Configuration(requirements, completedBindings);
            }
        }
    }
}
