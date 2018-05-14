using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// A simple implementation of <see cref="IBinding"/>.
    /// </summary>
    public class Binding : IBinding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <param name="bound">
        /// The bound <see cref="object"/>.
        /// </param>
        public Binding(IConfigurationRequirement requirement, object bound)
        {
            this.Requirement = requirement;
            this.Bound = bound;
        }

        /// <summary>
        /// The binding <see cref="IConfigurationRequirement"/>.
        /// </summary>
        public IConfigurationRequirement Requirement { get; private set; }

        /// <summary>
        /// The bound <see cref="object"/>.
        /// </summary>
        public object Bound { get; private set; }
    }
}
