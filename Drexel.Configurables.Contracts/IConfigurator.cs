using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Transforms mappings between <see cref="IConfigurationRequirement"/>s and <see cref="object"/>s to
    /// <see cref="IConfiguration"/>s.
    /// </summary>
    public interface IConfigurator
    {
        /// <summary>
        /// Returns an <see cref="IConfiguration"/> produced using the supplied <paramref name="mappings"/>.
        /// </summary>
        /// <param name="mappings">
        /// A set of mappings between <see cref="IConfigurationRequirement"/>s and <see cref="object"/>s.
        /// </param>
        /// <returns>
        /// An <see cref="IConfiguration"/> produced from the supplied <paramref name="mappings"/>.
        /// </returns>
        /// <exception cref="InvalidMappingsException">
        /// Occurs when the supplied <paramref name="mappings"/> are invalid for this <see cref="IConfigurator"/>.
        /// </exception>
        IConfiguration Configure(IReadOnlyDictionary<IConfigurationRequirement, object> mappings);
    }
}
