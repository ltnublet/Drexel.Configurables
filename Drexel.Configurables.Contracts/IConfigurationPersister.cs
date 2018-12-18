using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a persister that can store an <see cref="IConfiguration"/> to an external source.
    /// </summary>
    public interface IConfigurationPersister
    {
        /// <summary>
        /// Persists the supplied <see cref="IConfiguration"/> for external storage.
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> to persist.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the persist operation.
        /// </param>
        /// <returns>
        /// A <see cref=" Task"/> representing the operation.
        /// </returns>
        Task PersistAsync(
            IConfiguration configuration,
            CancellationToken cancellationToken = default);
    }
}
