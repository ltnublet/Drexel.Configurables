namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a mapping between a <see cref="IConfigurationRequirement"/> and an <see cref="object"/> which
    /// satisfies its requirements.
    /// </summary>
    public interface IMapping
    {
        /// <summary>
        /// The mapped <see cref="IConfigurationRequirement"/>.
        /// </summary>
        IConfigurationRequirement Requirement { get; }

        /// <summary>
        /// The mapped <see cref="object"/>.
        /// </summary>
        object Value { get; }
    }
}
