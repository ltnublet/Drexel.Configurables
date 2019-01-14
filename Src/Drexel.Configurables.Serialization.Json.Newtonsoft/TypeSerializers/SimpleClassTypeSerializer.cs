﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public abstract class SimpleClassTypeSerializer<T> : BaseClassTypeSerializer<T>
        where T : class
    {
        public SimpleClassTypeSerializer(ClassRequirementType<T> type)
            : base(type)
        {
        }

        public override bool SupportsWrites => true;

        public override async Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            IEnumerable<T?>? values,
            CancellationToken cancellationToken = default)
        {
            if (values == null)
            {
                await intermediary.Writer.WriteNullAsync(cancellationToken).ConfigureAwait(false);
                return;
            }

            await intermediary.Writer.WriteStartArrayAsync(cancellationToken).ConfigureAwait(false);

            foreach (T? value in values)
            {
                await this.SerializeAsync(intermediary, value, cancellationToken).ConfigureAwait(false);
            }

            await intermediary.Writer.WriteEndArrayAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
