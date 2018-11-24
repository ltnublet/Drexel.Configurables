using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Persists an <see cref="IPersistableConfiguration"/> for later restoration.
    /// </summary>
    public interface IPersister
    {
        /// <summary>
        /// Persists the specified <see cref="IPersistableConfiguration"/> <paramref name="configuration"/> for later
        /// restoration.
        /// </summary>
        /// <param name="configuration">
        /// The configuration to persist.
        /// </param>
        /// <param name="millisecondsTimeout">
        /// If persisting doesn't complete within this number of millisecond, it will be aborted. To never abort,
        /// supply a value of <code>-1</code>.
        /// </param>
        void Persist(
            IPersistableConfiguration configuration,
            int millisecondsTimeout = -1);

        /// <summary>
        /// Asynchronously persists the specified <see cref="IPersistableConfiguration"/>
        /// <paramref name="configuration"/> for later restoration.
        /// </summary>
        /// <param name="configuration">
        /// The configuration to persist.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the execution state of the persist operation.
        /// </returns>
        Task PersistAsync(
            IPersistableConfiguration configuration,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
