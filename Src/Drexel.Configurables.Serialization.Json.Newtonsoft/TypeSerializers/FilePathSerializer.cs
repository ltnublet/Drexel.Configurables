using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.External;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.TypeSerializers
{
    public sealed class FilePathSerializer : SimpleClassTypeSerializer<FilePath>
    {
        public FilePathSerializer(ClassRequirementType<FilePath> type)
            : base(type)
        {
        }

        public override async Task SerializeAsync(
            NewtonsoftIntermediary intermediary,
            FilePath value,
            CancellationToken cancellationToken = default)
        {
            await intermediary.Writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

            await intermediary.Writer.WritePropertyNameAsync(FieldNames.Path, cancellationToken).ConfigureAwait(false);
            await intermediary.Writer.WriteValueAsync(value.Path, cancellationToken).ConfigureAwait(false);

            await intermediary.Writer.WritePropertyNameAsync(FieldNames.CaseSensitive, cancellationToken).ConfigureAwait(false);
            await intermediary.Writer.WriteValueAsync(value.CaseSensitive, cancellationToken).ConfigureAwait(false);

            await intermediary.Writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        private static class FieldNames
        {
            public static string Path { get; } = "path";

            public static string CaseSensitive { get; } = "caseSensitive";
        }
    }
}
