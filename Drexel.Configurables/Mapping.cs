using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// A simple implementation of <see cref="IMapping{IConfigurationRequirement}"/>.
    /// </summary>
    public class Mapping : IMapping<IConfigurationRequirement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <param name="bound">
        /// The <see cref="object"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Occurs when <paramref name="requirement"/> is <see langword="null"/>.
        /// </exception>
        public Mapping(IConfigurationRequirement requirement, object bound)
        {
            this.Key = requirement ?? throw new ArgumentNullException(nameof(requirement));
            this.Value = bound;
        }

        /// <summary>
        /// Gets the mapped <see cref="IConfigurationRequirement"/>.
        /// </summary>
        public IConfigurationRequirement Key { get; private set; }

        /// <summary>
        /// Gets the mapped <see cref="object"/>.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">
        /// An <see cref="object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is an instance of <see cref="Mapping"/> and its
        /// <see cref="Mapping.Key"/> equals the value of this instance's <see cref="Mapping.Key"/>,
        /// and its <see cref="Mapping.Value"/> equals the value of this instance's <see cref="Mapping.Value"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis
            if (obj == null || !(obj is IMapping<IConfigurationRequirement> other))
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
            {
                return false;
            }

            return this.Key.Equals(other.Key)
                && (this.Value?.Equals(other.Value) ?? other.Value == null);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// The hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            const int nullValueHash = 0;

            // Intentionally allow overflows during hash calculation.
            unchecked
            {
                return this.Key.GetHashCode() + (this.Value?.GetHashCode() ?? nullValueHash);
            }
        }
    }
}
