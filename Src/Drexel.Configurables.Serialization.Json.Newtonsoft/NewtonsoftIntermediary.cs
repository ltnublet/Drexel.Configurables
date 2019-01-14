using Newtonsoft.Json;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft
{
    public sealed class NewtonsoftIntermediary
    {
        public NewtonsoftIntermediary(JsonTextWriter writer)
        {
            this.Writer = writer;
        }

        public JsonTextWriter Writer { get; }
    }
}
