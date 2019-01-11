﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Contracts
{
    public sealed class Configuration
    {
        private readonly IReadOnlyDictionary<Requirement, dynamic?> backingPairs; 

        internal Configuration(
            RequirementSet requirements,
            IDictionary<Requirement, dynamic?> mappings)
        {
            this.Requirements = requirements;
            this.backingPairs = new ReadOnlyDictionary<Requirement, dynamic?>(mappings);
        }

        public RequirementSet Requirements { get; }

        public bool Contains(Requirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            return this.backingPairs.ContainsKey(requirement);
        }

        public object? Get(Requirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (this.TryGet(requirement, out object? buffer))
            {
                return buffer;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public T? Get<T>(ClassRequirement<T> requirement)
            where T : class
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (this.TryGet(requirement, out T? buffer))
            {
                return buffer;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public T Get<T>(StructRequirement<T> requirement)
            where T : struct
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (this.TryGet(requirement, out T buffer))
            {
                return buffer;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public object? GetOrDefault(Requirement requirement, Func<object?> defaultValueFactory)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (defaultValueFactory == null)
            {
                throw new ArgumentNullException(nameof(defaultValueFactory));
            }

            if (this.TryGet(requirement, out object? buffer))
            {
                return buffer;
            }
            else
            {
                return defaultValueFactory.Invoke();
            }
        }

        public T? GetOrDefault<T>(ClassRequirement<T> requirement, Func<T?> defaultValueFactory)
            where T : class
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (defaultValueFactory == null)
            {
                throw new ArgumentNullException(nameof(defaultValueFactory));
            }

            if (this.TryGet<T>(requirement, out T? buffer))
            {
                return buffer;
            }
            else
            {
                return defaultValueFactory.Invoke();
            }
        }

        public T GetOrDefault<T>(StructRequirement<T> requirement, Func<T> defaultValueFactory)
            where T : struct
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (defaultValueFactory == null)
            {
                throw new ArgumentNullException(nameof(defaultValueFactory));
            }

            if (this.TryGet<T>(requirement, out T buffer))
            {
                return buffer;
            }
            else
            {
                return defaultValueFactory.Invoke();
            }
        }

        public bool TryGet(Requirement requirement, out object? value)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (this.backingPairs.TryGetValue(requirement, out dynamic? buffer))
            {
                value = buffer;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool TryGet<T>(ClassRequirement<T> requirement, out T? value)
            where T : class
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (this.backingPairs.TryGetValue(requirement, out dynamic? buffer))
            {
                value = buffer;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool TryGet<T>(StructRequirement<T> requirement, out T value)
            where T : struct
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

#pragma warning disable CS8600 // Contract for internal constructor is that dictionary value must always be legal.
            if (this.backingPairs.TryGetValue(requirement, out dynamic buffer))
#pragma warning restore CS8600
            {
                value = buffer;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
