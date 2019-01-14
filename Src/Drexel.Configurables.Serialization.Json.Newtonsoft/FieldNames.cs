namespace Drexel.Configurables.Serialization.Json.Newtonsoft
{
    internal static class FieldNames
    {
        public static string Version { get; } = "version";

        public static string Configuration { get; } = "configuration";

        public static string Types { get; } = "types";

        public static string Requirements { get; } = "requirements";

        public static string Values { get; } = "values";

        public static class RequirementTypeNames
        {
            public static string Id { get; } = "id";

            public static string Type { get; } = "type";
        }

        public static class RequirementNames
        {
            public static string Id { get; } = "id";

            public static string Type { get; } = "type";
        }

        public static class ValueNames
        {
            public static string Key { get; } = "key";

            public static string Value { get; } = "value";
        }
    }
}
