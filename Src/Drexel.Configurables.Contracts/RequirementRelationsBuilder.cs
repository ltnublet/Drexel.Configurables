using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts.Exceptions;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Builds <see cref="RequirementRelations"/> instances based upon the internal state of the builder.
    /// </summary>
    public sealed class RequirementRelationsBuilder
    {
        private readonly HashSet<Requirement> backingDependsOn;
        private readonly HashSet<Requirement> backingExclusiveWith;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementRelationsBuilder"/> class.
        /// </summary>
        public RequirementRelationsBuilder()
        {
            this.backingDependsOn = new HashSet<Requirement>();
            this.backingExclusiveWith = new HashSet<Requirement>();
        }

        /// <summary>
        /// Adds the specified <see cref="Requirement"/> to the set of dependencies.
        /// </summary>
        /// <param name="dependsOn">
        /// The requirement that is depended upon.
        /// </param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dependsOn"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="RequirementRelationsBuilderConflictException">
        /// Thrown when the specified requirement has already been added to the set of exclusivities.
        /// </exception>
        /// <exception cref="RequirementRelationsUnsatisfiableException">
        /// Thrown when the specified requirement exists somewhere in the dependency tree of the exclusivity set.
        /// </exception>
        public RequirementRelationsBuilder AddDependsOn(Requirement dependsOn)
        {
            if (dependsOn == null)
            {
                throw new ArgumentNullException(nameof(dependsOn));
            }

            if (this.backingExclusiveWith.Contains(dependsOn))
            {
                throw new RequirementRelationsBuilderConflictException(dependsOn, dependsOn);
            }
            else if (this
                .backingExclusiveWith
                .SelectMany(x => x.ExclusiveWith)
                .Flatten(x => x.DependsOn)
                .Contains(dependsOn))
            {
                throw new RequirementRelationsUnsatisfiableException(dependsOn);
            }

            this.backingDependsOn.Add(dependsOn);
            return this;
        }

        /// <summary>
        /// Adds the specified <see cref="Requirement"/> to the set of exclusivities.
        /// </summary>
        /// <param name="exclusiveWith">
        /// The requirement that is exclusive.
        /// </param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="exclusiveWith"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="RequirementRelationsBuilderConflictException">
        /// Thrown when the specified requirement has already been added to the set of dependencies.
        /// </exception>
        /// <exception cref="RequirementRelationsUnsatisfiableException">
        /// Thrown when the specified requirement exists somewhere in the dependency tree of the dependency set.
        /// </exception>
        public RequirementRelationsBuilder AddExclusiveWith(Requirement exclusiveWith)
        {
            if (exclusiveWith == null)
            {
                throw new ArgumentNullException(nameof(exclusiveWith));
            }

            if (this.backingDependsOn.Contains(exclusiveWith))
            {
                throw new RequirementRelationsBuilderConflictException(exclusiveWith, exclusiveWith);
            }
            else if (this
                .backingExclusiveWith
                .SelectMany(x => x.DependsOn)
                .Flatten(x => x.DependsOn)
                .Contains(exclusiveWith))
            {
                throw new RequirementRelationsUnsatisfiableException(exclusiveWith);
            }

            this.backingExclusiveWith.Add(exclusiveWith);
            return this;
        }

        /// <summary>
        /// Builds a <see cref="RequirementRelations"/> based on the internal state of the builder.
        /// </summary>
        /// <returns>
        /// A <see cref="RequirementRelations"/> instance.
        /// </returns>
        public RequirementRelations Build()
        {
            return new RequirementRelations(this.backingDependsOn, this.backingExclusiveWith);
        }
    }
}
