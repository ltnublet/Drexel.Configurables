namespace Drexel.Configurables.Contracts.Localization
{
#nullable enable
    public sealed class RequirementLocalization
    {
        public RequirementLocalization(string? name, string? description)
        {
            this.Name = name;
            this.Description = description;
        }

        public string? Name { get; }

        public string? Description { get; }
    }
#nullable disable
}
