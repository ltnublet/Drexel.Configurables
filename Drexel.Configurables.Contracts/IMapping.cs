namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a mapping between a <typeparamref name="T"/> and an <see cref="object"/> which
    /// satisfies its requirements.
    /// </summary>
    public interface IMapping<out T>
    {
        /// <summary>
        /// Gets the mapped <typeparamref name="T"/>.
        /// </summary>
        T Key { get; }

        /// <summary>
        /// Gets the mapped <see cref="object"/>.
        /// </summary>
        object Value { get; }
    }
}
