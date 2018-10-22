using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Restores persisted <see cref="IPersistableConfiguration"/>s.
    /// </summary>
    public interface IRestorer
    {
        /// <summary>
        /// Restores an <see cref="IPersistableConfiguration"/>.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// If restoration doesn't complete within this number of millisecond, it will be aborted. To never abort,
        /// supply a value of <code>-1</code>.
        /// </param>
        /// <returns>
        /// The restored <see cref="IPersistableConfiguration"/>.
        /// </returns>
        IPersistableConfiguration Restore(int millisecondsTimeout = -1);

        /// <summary>
        /// Asynchronously restores an <see cref="IPersistableConfiguration"/>.
        /// </summary>
        /// <param name="token">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the execution state of the restore operation.
        /// </returns>
        Task<IPersistableConfiguration> RestoreAsync(CancellationToken token = default(CancellationToken));
    }
}
