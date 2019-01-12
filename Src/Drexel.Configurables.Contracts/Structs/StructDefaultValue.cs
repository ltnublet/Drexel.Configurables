namespace Drexel.Configurables.Contracts.Structs
{
    /// <summary>
    /// Represents a default value for a <see langword="struct"/> type.
    /// </summary>
    /// <typeparam name="T">
    /// The <see langword="struct"/> type of the default value.
    /// </typeparam>
    public sealed class StructDefaultValue<T> : DefaultValue
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructDefaultValue{T}"/> class.
        /// </summary>
        public StructDefaultValue()
            : base(false)
        {
            this.Value = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructDefaultValue{T}"/> class.
        /// </summary>
        /// <param name="value">
        /// The default value.
        /// </param>
        public StructDefaultValue(T value)
            : base(true)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the default value, if one exists; otherwise, gets the default value for the underlying type of this
        /// default value.
        /// </summary>
        public new T Value { get; }

        /// <summary>
        /// Gets the internal backing default value.
        /// </summary>
        protected override object? BackingValue => this.Value;
    }
}
