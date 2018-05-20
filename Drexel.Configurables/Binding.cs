using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// A simple implementation of <see cref="IBinding"/>.
    /// </summary>
    public class Binding : IBinding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <param name="bound">
        /// The bound <see cref="object"/>.
        /// </param>
        public Binding(IConfigurationRequirement requirement, object bound)
        {
            this.Requirement = requirement ?? throw new ArgumentNullException(nameof(requirement));
            this.Bound = bound;
        }

        /// <summary>
        /// The binding <see cref="IConfigurationRequirement"/>.
        /// </summary>
        public IConfigurationRequirement Requirement { get; private set; }

        /// <summary>
        /// The bound <see cref="object"/>.
        /// </summary>
        public object Bound { get; private set; }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">
        /// An <see cref="object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is an instance of <see cref="Binding"/> and its
        /// <see cref="Binding.Requirement"/> equals the value of this instance's <see cref="Binding.Requirement"/>,
        /// and its <see cref="Binding.Bound"/> equals the value of this instance's <see cref="Binding.Bound"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis
            if (obj == null || !(obj is Binding other))
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
            {
                return false;
            }

            return this.Requirement.Equals(other.Requirement)
                && (this.Bound?.Equals(other.Bound) ?? other.Bound == null);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// The hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Requirement.GetHashCode() + (this.Bound?.GetHashCode() ?? 0);
        }
    }
}
