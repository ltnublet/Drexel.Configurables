using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a validated set of mappings between <see cref="IConfigurationRequirement"/>s and
    /// <see cref="object"/>s.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Has additional functionality.")]
    public interface IConfiguration : IEnumerable<IMapping>
    {
        /// <summary>
        /// Gets the <see cref="IConfigurator"/> which produced this <see cref="IConfiguration"/>.
        /// </summary>
        IConfigurator Configurator { get; }

        /// <summary>
        /// Gets the set of <see cref="IConfigurationRequirement"/>s contained by this <see cref="IConfiguration"/>.
        /// </summary>
        IReadOnlyList<IConfigurationRequirement> Keys { get; }

        /// <summary>
        /// Gets the <see cref="object"/> mapped to the specified <paramref name="requirement"/>.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> mapped to the specified <paramref name="requirement"/>.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Occurs when the <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is not contained
        /// by this <see cref="IConfiguration"/>.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1043:UseIntegralOrStringArgumentForIndexers",
            Justification = "Not relevant here.")]
        object this[IConfigurationRequirement requirement] { get; }

        /// <summary>
        /// If the specified <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is contained by
        /// this <see cref="IConfiguration"/>, returns the mapped <see cref="object"/>; otherwise, returns the result
        /// of invoking the supplied <paramref name="defaultValueFactory"/>.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <param name="defaultValueFactory">
        /// The default value factory.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> mapped to the specified <see cref="IConfigurationRequirement"/> if it is
        /// contained by this <see cref="IConfiguration"/>; otherwise, the value returned by
        /// <paramref name="defaultValueFactory"/>.
        /// </returns>
        object GetOrDefault(IConfigurationRequirement requirement, Func<object> defaultValueFactory);

        /// <summary>
        /// If this <see cref="IConfiguration"/> contains a mapping for the specified
        /// <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> and the mapped <see cref="object"/>
        /// is of type <typeparamref name="T"/>, <paramref name="result"/> is set to the mapped <see cref="object"/>
        /// and <see langword="true"/> is returned. Otherwise, <paramref name="result"/> is set to the return value of
        /// invoking the <paramref name="defaultValueFactory"/>, and <see langword="false"/> is returned.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <param name="defaultValueFactory">
        /// The default value factory.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <typeparam name="T">
        /// The expected <see cref="Type"/> of the <see cref="object"/> to return. If the specified
        /// <see cref="IConfigurationRequirement"/> is contained by this <see cref="IConfiguration"/>, but the mapped
        /// <see cref="object"/> is of a <see cref="Type"/> not compatible with <typeparamref name="T"/>, then the
        /// <paramref name="defaultValueFactory"/> will be invoked.
        /// </typeparam>
        /// <returns>
        /// <see langword="true"/> if the <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is
        /// contained by this <see cref="IConfiguration"/>, and <paramref name="result"/> is able to be
        /// set to the expected <see cref="Type"/> <typeparamref name="T"/>; otherwise, <see langword="false"/>.
        /// </returns>
        bool TryGetOrDefault<T>(
            IConfigurationRequirement requirement,
            Func<T> defaultValueFactory,
            out T result);
    }
}
