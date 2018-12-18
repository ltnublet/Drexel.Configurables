using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a restorer than can restore an <see cref="IConfiguration"/> from an external source.
    /// </summary>
    public interface IConfigurationRestorer
    {
        /// <summary>
        /// Restores an <see cref="IConfiguration"/> from an external source.
        /// </summary>
        /// <param name="requirementSet">
        /// The set of requirements the restored <see cref="IConfiguration"/> should use.
        /// </param>
        /// <param name="unpersistableRequirementsCallback">
        /// The callback to invoke for requirements which were part of the supplied <paramref name="requirementSet"/>,
        /// but which cannot be restored.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the persist operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task{T}"/> of type <see cref="IConfiguration"/> representing the operation.
        /// </returns>
        Task<IConfiguration> RestoreAsync(
            IReadOnlyCollection<IRequirement> requirementSet,
            Func<IReadOnlyCollection<IRequirement>, IReadOnlyDictionary<IRequirement, object>> unpersistableRequirementsCallback,
            CancellationToken cancellationToken = default);
    }
}
