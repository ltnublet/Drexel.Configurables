using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a required requirement is missing.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "Unnecessary.")]
    public class MissingRequiredRequirementException : ConfigurationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingRequiredRequirementException"/> class.
        /// </summary>
        /// <param name="missingRequirement">
        /// The required requirement that was missing.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is illegally <see langword="null"/>.
        /// </exception>
        public MissingRequiredRequirementException(IRequirement missingRequirement)
            : base("Required requirement is missing.")
        {
            this.MissingRequirement = missingRequirement
                ?? throw new ArgumentNullException(nameof(missingRequirement));
        }

        /// <summary>
        /// Gets the required requirement that was missing.
        /// </summary>
        public IRequirement MissingRequirement { get; }
    }
}
