namespace Drexel.Configurables.Contracts.Classes
{
    /// <summary>
    /// Represents a default value for a <see langword="class"/> type.
    /// </summary>
    /// <typeparam name="T">
    /// The <see langword="class"/> type of the default value.
    /// </typeparam>
    public sealed class ClassDefaultValue<T> : DefaultValue
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDefaultValue{T}"/> class.
        /// </summary>
        public ClassDefaultValue()
            : base(false)
        {
            this.Value = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDefaultValue{T}"/> class.
        /// </summary>
        /// <param name="value">
        /// The default value.
        /// </param>
        public ClassDefaultValue(T? value)
            : base(true)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the default value, if one exists; otherwise, gets the default value for the underlying type of this
        /// default value.
        /// </summary>
        public new T? Value { get; }

        /// <summary>
        /// Gets the internal backing default value.
        /// </summary>
        protected override object? BackingValue => this.Value;
    }
}
