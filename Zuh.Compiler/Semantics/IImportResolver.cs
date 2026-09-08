namespace Zuh.Compiler.Semantics {
    /// <summary>
    /// dependency injection based file system go.
    /// </summary>
    /// <remarks>
    /// one of the main reasons i started this project was to use a dependency injection based module resolver.
    /// this takes a lot of inspiration from how Jint handles it.
    /// </remarks>
    public interface IImportResolver {
        /// <summary>
        /// you have one job.
        /// </summary>
        /// <param name="sourceId">unit id of the unit trying to import a module.</param>
        /// <param name="module">the name of the module to import.</param>
        /// <returns>data about the resolved import.</returns>
        public IImportResolution ResolveImport(string sourceId, string module);
    }
}