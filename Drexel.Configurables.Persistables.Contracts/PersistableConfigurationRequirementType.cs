using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Represents a persistable configuration type.
    /// </summary>
    public class PersistableConfigurationRequirementType : IConfigurationRequirementType
    {
        private readonly IConfigurationRequirementType type;
        private readonly Func<object, string> encodeFunc;
        private readonly Func<string, object> restoreFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistableConfigurationRequirementType"/> class.
        /// </summary>
        /// <param name="id">
        /// The unique ID that identifies this requirement type.
        /// </param>
        /// <param name="version">
        /// The <see cref="Version"/> of the requirement type.
        /// </param>
        /// <param name="type">
        /// The underlying <see cref="IConfigurationRequirementType"/>.
        /// </param>
        /// <param name="encodeFunc">
        /// The encoding function to convert a value of this type to a <see langword="string"/>.
        /// </param>
        /// <param name="restoreFunc">
        /// The restoration function to convert a <see langword="string"/> to a value of this type.
        /// </param>
        public PersistableConfigurationRequirementType(
            Guid id,
            Version version,
            IConfigurationRequirementType type,
            Func<object, string> encodeFunc,
            Func<string, object> restoreFunc)
        {
            this.Id = id;
            this.Version = version ?? throw new ArgumentNullException(nameof(version));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.encodeFunc = encodeFunc ?? throw new ArgumentNullException(nameof(encodeFunc));
            this.restoreFunc = restoreFunc ?? throw new ArgumentNullException(nameof(restoreFunc));
        }

        /// <summary>
        /// Gets the unique ID of this requirement type.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the associated <see cref="IConfigurationRequirementType"/>.
        /// </summary>
        public Type Type => this.type.Type;

        /// <summary>
        /// Gets the <see cref="System.Version"/> of this requirement type.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Encodes the supplied <see langword="object"/> <paramref name="value"/> to a <see langword="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see langword="object"/> value to encode.
        /// </param>
        /// <returns>
        /// A <see langword="string"/> encoding of the supplied <see langword="object"/> value.
        /// </returns>
        public string Encode(object value) => this.encodeFunc.Invoke(value);

        /// <summary>
        /// Restores the supplied <see langword="string"/> <paramref name="value"/> to an <see langword="object"/>
        /// of the <see cref="System.Type"/> of this <see cref="IConfigurationRequirementType"/>.
        /// </summary>
        /// <param name="value">
        /// The <see langword="string"/> <paramref name="value"/> to restore.
        /// </param>
        /// <returns>
        /// An <see langword="object"/> of the <see langword="System.Type"/> of this
        /// <see cref="IConfigurationRequirementType"/>.
        /// </returns>
        public object Restore(string value) => this.restoreFunc.Invoke(value);
    }
}
