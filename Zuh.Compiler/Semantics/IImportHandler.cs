using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics {
    /// <summary>
    /// dependency injection based file system go.
    /// </summary>
    /// <remarks>
    /// one of the main reasons i started this project was to use a dependency injection based module resolver.
    /// this takes a lot of inspiration from how Jint handles it.
    /// </remarks>
    public interface IImportHandler {
        public ImportResolution ResolveModule(string sourceId, string module);
        public ZuhFile FetchContent(ImportResolution resolution);
    }
}