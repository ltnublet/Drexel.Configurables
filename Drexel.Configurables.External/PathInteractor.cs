using System.IO;

namespace Drexel.Configurables.External
{
    /// <summary>
    /// A concrete implementation of <see cref="IPathInteractor"/> using <see cref="System.IO.Path"/> as the underlying
    /// mechanism by which to provide path services.
    /// </summary>
    public class PathInteractor : IPathInteractor
    {
        /// <summary>
        /// Returns the absolute path for the specified <paramref name="path"/> <see cref="string"/>.
        /// </summary>
        /// <param name="path">
        /// The file or directory for which to obtain absolute path information.
        /// </param>
        /// <returns>
        /// The fully qualified location of <paramref name="path"/>.
        /// </returns>
        public string GetFullPath(string path)
        {
            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return null;
            }
        }

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
        public bool IsPathRooted(string path)
        {
            return Path.IsPathRooted(path);
        }
    }
}