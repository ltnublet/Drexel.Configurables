using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// Validates a supplied <see cref="object"/>.
    /// </summary>
    /// <param name="instance">
    /// The <see cref="object"/> to validate.
    /// </param>
    /// <param name="requirement">
    /// The <see cref="IConfigurationRequirement"/> being validated.
    /// </param>
    /// <param name="dependentMappings">
    /// The set of mappings upon which this validation is dependent, and the associated <b>validated</b> values.
    /// </param>
    /// <returns>
    /// <see langword="null"/> if the object passed validation; else, an <see cref="Exception"/> describing why the
    /// supplied <paramref name="instance"/> failed validation.
    /// </returns>
    public delegate Exception Validator(
        object instance,
        IConfigurationRequirement requirement,
        IConfiguration dependentMappings);

    /// <summary>
    /// A simple <see cref="IConfigurationRequirement"/> implementation.
    /// </summary>
    public class ConfigurationRequirement : IConfigurationRequirement
    {
#pragma warning disable SA1516 // Elements must be separated by blank line
        private const string SuppliedCollectionOfInvalidSize =
            "Supplied object is a collection of invalid size. Number of items: '{0}'. Minimum: '{1}'. Maximum: '{2}'.";
        private const string SuppliedCollectionContainsObjectsOfWrongType =
            "Supplied collection contains object(s) of wrong type. Expected type: '{0}'.";
        private const string SuppliedObjectIsOfWrongType = "Supplied object is of wrong type. Expected type: '{0}'.";
#pragma warning restore SA1516 // Elements must be separated by blank line
        private const string SuppliedObjectIsNotIEnumerable = "Supplied object is not an IEnumerable.";
        private const string StringMustBeNonWhitespace = "String must not be whitespace.";

        private readonly Validator validator;
        private string cachedToString;

        /// <summary>
        /// Instantiates a new <see cref="ConfigurationRequirement"/> instance using the supplied parameters.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="ofType">
        /// The <see cref="ConfigurationRequirementType"/> of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether this <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="validator">
        /// Validates <see cref="object"/>s to determine if they satisfy the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        public ConfigurationRequirement(
            string name,
            string description,
            ConfigurationRequirementType ofType,
            bool isOptional,
            Validator validator,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            ConfigurationRequirement.ThrowIfBadString(name, nameof(name));
            ConfigurationRequirement.ThrowIfBadString(description, nameof(description));
            this.Name = name;
            this.Description = description;
            this.OfType = ofType;
            this.IsOptional = isOptional;
            this.CollectionInfo = collectionInfo;
            this.DependsOn = dependsOn ?? new IConfigurationRequirement[0];
            this.ExclusiveWith = exclusiveWith ?? new IConfigurationRequirement[0];

            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.cachedToString = null;
        }

        /// <summary>
        /// The set of <see cref="IConfigurationRequirement"/>s which must be supplied alongside this requirement.
        /// </summary>
        public IEnumerable<IConfigurationRequirement> DependsOn { get; private set; }

        /// <summary>
        /// The description of this requirement.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The set of <see cref="IConfigurationRequirement"/>s which must not be supplied alongside this requirement.
        /// </summary>
        public IEnumerable<IConfigurationRequirement> ExclusiveWith { get; private set; }

        /// <summary>
        /// <see langword="null"/> if this requirement expects a single instance of
        /// <see cref="ConfigurationRequirementType"/> <see cref="OfType"/>; else, the constraints of the required
        /// collection are described by the <see cref="CollectionInfo"/>.
        /// </summary>
        public CollectionInfo CollectionInfo { get; private set; }

        /// <summary>
        /// <see langword="true"/> if this requirement is optional; <see langword="false"/> if this requirement is
        /// required.
        /// </summary>
        public bool IsOptional { get; private set; }

        /// <summary>
        /// The type of this requirement. This indicates what the expected <see cref="Type"/> of the input to
        /// <see cref="Validate(object, IConfiguration)"/> is.
        /// </summary>
        public ConfigurationRequirementType OfType { get; private set; }

        /// <summary>
        /// The name of this requirement.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.String"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <param name="additionalValidation">
        /// A <see cref="Validator"/> which accepts the instance <see cref="object"/> being validated. This
        /// <paramref name="additionalValidation"/> will only be invoked if the <see cref="object"/> passes the
        /// default validation logic.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.String"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement String(
            string name,
            string description,
            bool isOptional = false,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null,
            Validator additionalValidation = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.String,
                isOptional,
                ConfigurationRequirement.CreateSimpleValidator(
                    ConfigurationRequirementType.String,
                    additionalValidation),
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.SecureString"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <param name="additionalValidation">
        /// A <see cref="Validator"/> which accepts the instance <see cref="object"/> being validated. This
        /// <paramref name="additionalValidation"/> will only be invoked if the <see cref="object"/> passes the
        /// default validation logic.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.SecureString"/>
        /// with the supplied properties.
        /// </returns>
        public static IConfigurationRequirement SecureString(
            string name,
            string description,
            bool isOptional = false,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null,
            Validator additionalValidation = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.SecureString,
                isOptional,
                ConfigurationRequirement.CreateSimpleValidator(
                    ConfigurationRequirementType.SecureString,
                    additionalValidation),
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.FilePath"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <param name="additionalValidation">
        /// A <see cref="Validator"/> which accepts the instance <see cref="object"/> being validated. This
        /// <paramref name="additionalValidation"/> will only be invoked if the <see cref="object"/> passes the
        /// default validation logic.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.FilePath"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement FilePath(
            string name,
            string description,
            bool isOptional = false,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null,
            Validator additionalValidation = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.FilePath,
                isOptional,
                ConfigurationRequirement.CreateSimpleValidator(
                    ConfigurationRequirementType.FilePath,
                    additionalValidation),
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.Int32"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <param name="additionalValidation">
        /// A <see cref="Validator"/> which accepts the instance <see cref="object"/> being validated. This
        /// <paramref name="additionalValidation"/> will only be invoked if the <see cref="object"/> passes the
        /// default validation logic.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.Int32"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement Int32(
            string name,
            string description,
            bool isOptional = false,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null,
            Validator additionalValidation = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.Int32,
                isOptional,
                ConfigurationRequirement.CreateSimpleValidator(
                    ConfigurationRequirementType.Int32,
                    additionalValidation),
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.Int64"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <param name="additionalValidation">
        /// A <see cref="Validator"/> which accepts the instance <see cref="object"/> being validated. This
        /// <paramref name="additionalValidation"/> will only be invoked if the <see cref="object"/> passes the
        /// default validation logic.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.Int64"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement Int64(
            string name,
            string description,
            bool isOptional = false,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null,
            Validator additionalValidation = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.Int64,
                isOptional,
                ConfigurationRequirement.CreateSimpleValidator(
                    ConfigurationRequirementType.Int64,
                    additionalValidation),
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.Bool"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <param name="additionalValidation">
        /// A <see cref="Validator"/> which accepts the instance <see cref="object"/> being validated. This
        /// <paramref name="additionalValidation"/> will only be invoked if the <see cref="object"/> passes the
        /// default validation logic.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.Bool"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement Bool(
            string name,
            string description,
            bool isOptional = false,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null,
            Validator additionalValidation = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.Bool,
                isOptional,
                ConfigurationRequirement.CreateSimpleValidator(
                    ConfigurationRequirementType.Bool,
                    additionalValidation),
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.Uri"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <param name="additionalValidation">
        /// A <see cref="Validator"/> which accepts the instance <see cref="object"/> being validated. This
        /// <paramref name="additionalValidation"/> will only be invoked if the <see cref="object"/> passes the
        /// default validation logic.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.Uri"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement Uri(
            string name,
            string description,
            bool isOptional = false,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null,
            Validator additionalValidation = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.Uri,
                isOptional,
                ConfigurationRequirement.CreateSimpleValidator(
                    ConfigurationRequirementType.Uri,
                    additionalValidation),
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a <see cref="Validator"/> which performs a simple validation for the specified
        /// <see cref="ConfigurationRequirementType"/> <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The <see cref="ConfigurationRequirementType"/> to perform validation using.
        /// </param>
        /// <param name="additionalValidation">
        /// A <see cref="Validator"/> which accepts the instance <see cref="object"/> being validated. This
        /// <paramref name="additionalValidation"/> will only be invoked if the <see cref="object"/> passes the
        /// default validation logic.
        /// </param>
        /// <returns>
        /// A simple <see cref="Validator"/> with the default validation logic.
        /// </returns>
        public static Validator CreateSimpleValidator(
            ConfigurationRequirementType type,
            Validator additionalValidation = null)
        {
            return (instance, requirement, dependentMappings) => ConfigurationRequirement.SimpleValidator(
                type,
                instance,
                requirement,
                dependentMappings,
                additionalValidation);
        }

        /// <summary>
        /// Validates the supplied <see cref="object"/> for this requirement.
        /// </summary>
        /// <param name="instance">
        /// The <see cref="object"/> to perform validation upon.
        /// </param>
        /// <param name="dependentMappings">
        /// An <see cref="IConfiguration"/> containing <see cref="IMapping"/>s for all
        /// <see cref="IConfigurationRequirement"/>s in this requirement's
        /// <see cref="IConfigurationRequirement.DependsOn"/>.
        /// </param>
        /// <returns>
        /// <see langword="null"/> if the supplied <see cref="object"/> <paramref name="instance"/> passed validation;
        /// an <see cref="Exception"/> describing the validation failure otherwise.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "By design, no exception should escape the validation call.")]
        public Exception Validate(object instance, IConfiguration dependentMappings = null)
        {
            try
            {
                return this.validator.Invoke(instance, this, dependentMappings);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="ConfigurationRequirement"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the current <see cref="ConfigurationRequirement"/>.
        /// </returns>
        public override string ToString()
        {
            const string newline = "\r\n";
            const string @null = "null";

            string JsonEscape(string toEscape)
            {
                StringBuilder builder = new StringBuilder();
                foreach (char @char in toEscape)
                {
                    switch (@char)
                    {
                        case '\b':
                            builder.Append(@"\b");
                            break;
                        case '\f':
                            builder.Append(@"\f");
                            break;
                        case '\n':
                            builder.Append(@"\n");
                            break;
                        case '\r':
                            builder.Append(@"\r");
                            break;
                        case '\t':
                            builder.Append(@"\t");
                            break;
                        case '\"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append(@"\");
                            break;
                        default:
                            builder.Append(@char);
                            break;
                    }
                }

                return builder.ToString();
            }

            void AppendString(StringBuilder builder, string name, string value)
            {
                builder.Append("\t\"");
                builder.Append(name);
                builder.Append("\": ");

                if (value == null)
                {
                    builder.Append(@null);
                }
                else
                {
                    builder.Append('"');
                    builder.Append(JsonEscape(value));
                    builder.Append('"');
                }
            }

            void AppendBool(StringBuilder builder, string name, bool value)
            {
                builder.Append("\t\"");
                builder.Append(name);
                builder.Append("\": ");
                builder.Append(value ? "true" : "false");
            }

            void AppendArray(StringBuilder builder, string name, string[] values)
            {
                builder.Append("\t\"");
                builder.Append(name);
                builder.Append("\": [");

                if (values.Length == 0)
                {
                    builder.Append("]");
                }
                else
                {
                    builder.Append(newline);

                    for (int index = 0; index < values.Length; index++)
                    {
                        builder.Append("\t\t");
                        if (values[index] == null)
                        {
                                builder.Append(@null);
                        }
                        else
                        {
                            builder.Append('"');
                            builder.Append(JsonEscape(values[index]));
                            builder.Append("\"");

                            if (index != values.Length - 1)
                            {
                                builder.Append(",");
                            }

                            builder.Append(newline);
                        }
                    }

                    builder.Append("\t]");
                }
            }

#pragma warning disable SA1009 // Closing parenthesis must be spaced correctly
            void AppendObject(StringBuilder builder, string name, IEnumerable<(string Name, string Value)> values)
#pragma warning restore SA1009 // Closing parenthesis must be spaced correctly
            {
                builder.Append("\t\"");
                builder.Append(name);
                builder.Append("\": ");
                if (values == null)
                {
                    builder.Append(@null);
                    return;
                }

                builder.Append("{");
                builder.Append(newline);

                foreach ((string Name, string Value) value in values)
                {
                    builder.Append("\t\t\"");
                    builder.Append(value.Name);
                    builder.Append("\": ");
                    builder.Append(value.Value);
                    builder.Append(",");
                    builder.Append(newline);
                }

                builder.Append("\t}");
            }

            if (this.cachedToString == null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append('{');

                builder.Append(newline);
                AppendString(builder, nameof(this.Name), this.Name);
                builder.Append(',');
                builder.Append(newline);
                AppendString(builder, nameof(this.OfType), this.OfType.Type.FullName);
                builder.Append(',');
                builder.Append(newline);
                AppendBool(builder, nameof(this.IsOptional), this.IsOptional);
                builder.Append(',');
                builder.Append(newline);
                AppendString(builder, nameof(this.Description), this.Description);
                builder.Append(',');
                builder.Append(newline);
                AppendObject(
                    builder,
                    nameof(this.CollectionInfo),
                    this.CollectionInfo == null
                        ? null
                        : new(string Name, string Value)[]
#pragma warning disable SA1009 // Closing parenthesis must be spaced correctly
                            {
                                (
                                    nameof(this.CollectionInfo.MaximumCount),
                                    this.CollectionInfo.MaximumCount.HasValue
                                        ? '"'
                                            + this
                                                .CollectionInfo
                                                .MaximumCount
                                                .Value
                                                .ToString(CultureInfo.InvariantCulture)
                                            + '"'
                                        : @null
                                ),
                                (
                                    nameof(this.CollectionInfo.MinimumCount),
                                    '"' + this.CollectionInfo.MinimumCount.ToString(CultureInfo.InvariantCulture) + '"'
                                )
#pragma warning restore SA1009 // Closing parenthesis must be spaced correctly
                            });
                builder.Append(',');
                builder.Append(newline);
                AppendArray(
                    builder,
                    nameof(this.DependsOn),
                    this.DependsOn.Select(x => "\"" + (x?.Name ?? @null) + "\"").ToArray());
                builder.Append(',');
                builder.Append(newline);
                AppendArray(
                    builder,
                    nameof(this.ExclusiveWith),
                    this.ExclusiveWith.Select(x => "\"" + (x?.Name ?? @null) + "\"").ToArray());
                builder.Append(newline);

                builder.Append('}');

                this.cachedToString = builder.ToString();
            }

            return this.cachedToString;
        }

        /// <summary>
        /// This method is only internal so that unit tests can reach it. Do not call it directly unless the caller is
        /// within <see cref="ConfigurationRequirement"/>.
        /// </summary>
        /// <param name="type">
        /// This field intentionally left blank.
        /// </param>
        /// <param name="instance">
        /// This field intentionally left blank.
        /// </param>
        /// <param name="requirement">
        /// This field intentionally left blank.
        /// </param>
        /// <param name="dependentMappings">
        /// This field intentionally left blank.
        /// </param>
        /// <param name="additionalValidation">
        /// This field intentionally left blank.
        /// </param>
        /// <returns>
        /// This field intentionally left blank.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We don't control what types of exceptions validation can raise.")]
        internal static Exception SimpleValidator(
            ConfigurationRequirementType type,
            object instance,
            IConfigurationRequirement requirement,
            IConfiguration dependentMappings,
            Validator additionalValidation = null)
        {
            if (instance == null)
            {
                return new ArgumentNullException(nameof(instance));
            }
            else if (requirement.CollectionInfo == null)
            {
                if (!type.Type.IsAssignableFrom(instance.GetType()))
                {
                    return new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            ConfigurationRequirement.SuppliedObjectIsOfWrongType,
                            type.Type.ToString()),
                        nameof(instance));
                }
            }
            else if (requirement.CollectionInfo != null)
            {
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis
                if (!(instance is IEnumerable enumerable))
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
                {
                    return new ArgumentException(
                        ConfigurationRequirement.SuppliedObjectIsNotIEnumerable,
                        nameof(instance));
                }

                object[] array = enumerable.Cast<object>().ToArray();
                if (array.Length < requirement.CollectionInfo.MinimumCount
                    || (requirement.CollectionInfo.MaximumCount.HasValue
                        ? array.Length > requirement.CollectionInfo.MaximumCount.Value
                        : false))
                {
                    return new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            ConfigurationRequirement.SuppliedCollectionOfInvalidSize,
                            array.Length,
                            requirement.CollectionInfo.MinimumCount,
                            requirement.CollectionInfo.MaximumCount.HasValue
                                ? requirement.CollectionInfo.MaximumCount.Value.ToString(CultureInfo.InvariantCulture)
                                : "null"),
                        nameof(instance));
                }

                if (array.Any(x => !type.Type.IsAssignableFrom(x.GetType())))
                {
                    return new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            ConfigurationRequirement.SuppliedCollectionContainsObjectsOfWrongType,
                            type.Type.ToString()),
                        nameof(instance));
                }
            }

            try
            {
                return additionalValidation?.Invoke(instance, requirement, dependentMappings);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        [DebuggerHidden]
        private static void ThrowIfBadString(string @string, string name)
        {
            if (@string == null)
            {
                throw new ArgumentNullException(name);
            }
            else if (string.IsNullOrWhiteSpace(@string))
            {
                throw new ArgumentException(ConfigurationRequirement.StringMustBeNonWhitespace, name);
            }
        }
    }
}
