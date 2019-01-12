namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents the default value, if one exists.
    /// </summary>
    public abstract class DefaultValue
    {
        private protected DefaultValue(bool hasValue)
        {
            this.HasValue = hasValue;
        }

        /// <summary>
        /// Gets a value indicating whether a default value exists.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Gets the default value if one exists; otherwise, gets the default value for the underlying type of the
        /// requirement.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Gets the internal backing default value.
        /// </summary>
        protected abstract object? BackingValue { get; }
    }
}
