using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using Drexel.Configurables.External;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a configuration type.
    /// </summary>
    public sealed class ConfigurationRequirementType
    {
        /// <summary>
        /// Static initializer for the <see cref="ConfigurationRequirementType"/> class; populates properties in order
        /// so that reflection can be used to retrieve the total set of supported types.
        /// </summary>
        static ConfigurationRequirementType()
        {
            ConfigurationRequirementType.Bool = new ConfigurationRequirementType(typeof(Boolean));
            ConfigurationRequirementType.FilePath = new ConfigurationRequirementType(typeof(FilePath));
            ConfigurationRequirementType.Int32 = new ConfigurationRequirementType(typeof(Int32));
            ConfigurationRequirementType.Int64 = new ConfigurationRequirementType(typeof(Int64));
            ConfigurationRequirementType.SecureString = new ConfigurationRequirementType(typeof(SecureString));
            ConfigurationRequirementType.String = new ConfigurationRequirementType(typeof(String));
            ConfigurationRequirementType.Uri = new ConfigurationRequirementType(typeof(Uri));

            // Programmatically build the list of supported types by iterating over all public static fields which are
            // of type ConfigurationRequirementType. All properties must be initialized before this is executed.
            ConfigurationRequirementType.Types =
                typeof(ConfigurationRequirementType)
                    .GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => x.PropertyType == typeof(ConfigurationRequirementType))
                    .Select(x => x.GetValue(null, null))
                    .Cast<ConfigurationRequirementType>()
                    .ToList();
        }

        /// <summary>
        /// Instantiates a new <see cref="ConfigurationRequirementType"/> of the specified <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">
        /// The <see cref="System.Type"/> of the <see cref="ConfigurationRequirementType"/>.
        /// </param>
        public ConfigurationRequirementType(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see cref="Boolean"/>.
        /// </summary>
        public static ConfigurationRequirementType Bool { get; }

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type
        /// <see cref="Drexel.Configurables.External.FilePath"/>.
        /// </summary>
        public static ConfigurationRequirementType FilePath { get; }

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see cref="Int32"/>.
        /// </summary>
        public static ConfigurationRequirementType Int32 { get; }

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see cref="Int64"/>.
        /// </summary>
        public static ConfigurationRequirementType Int64 { get; }

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type
        /// <see cref="System.Security.SecureString"/>.
        /// </summary>
        public static ConfigurationRequirementType SecureString { get; }

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see cref="String"/>.
        /// </summary>
        public static ConfigurationRequirementType String { get; }

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see cref="System.Uri"/>.
        /// </summary>
        public static ConfigurationRequirementType Uri { get; }

        /// <summary>
        /// The set of default supported <see cref="ConfigurationRequirementType"/>s. This does not include types
        /// instantiated in user code: only those statically defined on <see cref="ConfigurationRequirementType"/>
        /// (ex. <see cref="ConfigurationRequirementType.String"/>).
        /// </summary>
        public static IReadOnlyList<ConfigurationRequirementType> Types { get; }

        /// <summary>
        /// The <see cref="System.Type"/> of the associated <see cref="IConfigurationRequirement"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "The distinction between the inherited method GetType and the property Type is clear.")]
        public Type Type { get; private set; }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">
        /// An <see cref="object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is an instance of
        /// <see cref="ConfigurationRequirementType"/> and its <see cref="ConfigurationRequirementType.Type"/> equals
        /// the value of this instance's <see cref="ConfigurationRequirementType.Type"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ConfigurationRequirementType other))
            {
                return false;
            }

            return this.Type == other.Type;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// The hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }
    }
}
