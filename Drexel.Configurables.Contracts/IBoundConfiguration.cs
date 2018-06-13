using System;
using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a configuration applicable to an <see cref="IConfigurable"/>.
    /// </summary>
    public interface IBoundConfiguration
    {
        /// <summary>
        /// A collection of <see cref="IBinding"/>s contained by this <see cref="IBoundConfiguration"/>.
        /// </summary>
        IEnumerable<IBinding> Bindings { get; }

        /// <summary>
        /// Gets the <see cref="object"/> associated with the specified <see cref="IConfigurationRequirement"/>
        /// <paramref name="requirement"/>.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> associated with the specified <see cref="IConfigurationRequirement"/>
        /// <paramref name="requirement"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1043:UseIntegralOrStringArgumentForIndexers",
            Justification = "This class is a data source.")]
        object this[IConfigurationRequirement requirement] { get; }

        /// <summary>
        /// If the specified <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is contained by
        /// this <see cref="IBoundConfiguration"/>, returns the <see cref="object"/> held by the associated
        /// <see cref="IBinding"/> in <see cref="IBoundConfiguration.Bindings"/>; otherwise, returns the result of
        /// invoking the supplied <paramref name="defaultValueFactory"/>.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <param name="defaultValueFactory">
        /// The default value factory.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> associated with the specified <see cref="IConfigurationRequirement"/> if it is
        /// contained by this <see cref="IBoundConfiguration"/>; otherwise, the value returned by
        /// <paramref name="defaultValueFactory"/>.
        /// </returns>
        object GetOrDefault(IConfigurationRequirement requirement, Func<object> defaultValueFactory);

        /// <summary>
        /// If the specified <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is contained by
        /// this <see cref="IBoundConfiguration"/>, <paramref name="result"/> is set to the <typeparamref name="T"/>
        /// held by the associated <see cref="IBinding"/> in <see cref="IBoundConfiguration.Bindings"/>, and
        /// <see langword="true"/> is returned; otherwise, <paramref name="result"/> is set to the result of
        /// invoking the supplied <paramref name="defaultValueFactory"/>, and <see langword="false"/> is returned.
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
        /// <see cref="IConfigurationRequirement"/> is contained by this <see cref="IBoundConfiguration"/>, but the
        /// <paramref name="result"/> cannot be set to the expected <see cref="Type"/> <typeparamref name="T"/>, then
        /// the <paramref name="defaultValueFactory"/> will be invoked.
        /// </typeparam>
        /// <returns>
        /// <see langword="true"/> if the <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is
        /// contained by this <see cref="IBoundConfiguration"/>, and <paramref name="result"/> is able to be
        /// set to the expected <see cref="Type"/> <typeparamref name="T"/>; otherwise, return <see langword="false"/>.
        /// </returns>
        bool TryGetOrDefault<T>(IConfigurationRequirement requirement, Func<T> defaultValueFactory, out T result);
    }
}
