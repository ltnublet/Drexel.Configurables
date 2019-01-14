using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public abstract class BaseClassTypeSerializer<T> : IClassTypeSerializer<T, NewtonsoftIntermediary>
        where T : class
    {
        private readonly ClassRequirementType<T> type;

        public BaseClassTypeSerializer(ClassRequirementType<T> type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public abstract bool SupportsWrites { get; }

        public abstract Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            T? value,
            CancellationToken cancellationToken = default);

        public abstract Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            IEnumerable<T?>? values,
            CancellationToken cancellationToken = default);

        public async Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            object? value,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (intermediary == null)
            {
                throw new ArgumentNullException(nameof(intermediary));
            }

            if (this.type.TryCast(value, out T? buffer))
            {
                await this.SerializeAsync(intermediary, buffer, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public async Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            IEnumerable? values,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (intermediary == null)
            {
                throw new ArgumentNullException(nameof(intermediary));
            }

            if (this.type.TryCast(values, out IEnumerable<T?>? buffer))
            {
                await this.SerializeAsync(intermediary, buffer, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
