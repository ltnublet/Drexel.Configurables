namespace Drexel.Configurables.Json.V1
{
    internal static class FieldNames
    {
        public static string Keys { get; } = "keys";

        public static string Types { get; } = "types";

        public static string Values { get; } = "values";

        public static string Version { get; } = "version";

        public static class KeyFieldNames
        {
            public static string Id { get; } = "id";

            public static string TypeId { get; } = "typeId";
        }

        public static class TypeFieldNames
        {
            public static string Id { get; } = "id";

            public static string Version { get; } = "version";
        }
    }
}
