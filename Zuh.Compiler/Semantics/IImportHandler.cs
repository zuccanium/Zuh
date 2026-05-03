using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics {
    public interface IImportHandler {
        public ImportResolution ResolveModule(string sourceId, string module);
        public ZuhFile FetchContent(ImportResolution resolution);
    }
}