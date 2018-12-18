namespace Drexel.Configurables.External
{
    /// <summary>
    /// Provides file path services.
    /// </summary>
    public interface IPathInteractor
    {
        /// <summary>
        /// Returns the absolute path for the specified <paramref name="path"/> <see cref="string"/>.
        /// </summary>
        /// <param name="path">
        /// The file or directory for which to obtain absolute path information.
        /// </param>
        /// <returns>
        /// The fully qualified location of <paramref name="path"/>, or <see langword="null"/> if the absolute path
        /// cannot be retrieved.
        /// </returns>
        string? GetFullPath(string path);

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the specified <paramref name="path"/> <see cref="string"/>
        /// contains a root.
        /// </summary>
        /// <param name="path">
        /// The path to test.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if path contains a root; otherwise, <see langword="false"/>.
        /// </returns>
        bool IsPathRooted(string path);
    }
}