using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics;

namespace Zuh.Compiler.Tests.Semantics {
    public class MockImportResolver : IImportResolver {
        public Dictionary<string, ZuhFile> Files { get; private init; } = [];

        public IImportResolution ResolveImport(string sourceId, string module) {
            var actualPath = Path.Join(Path.GetDirectoryName(sourceId), module);

            if(Files.TryGetValue(actualPath, out var file))
                return new MockImportResolution() {
                    Success = true,
                    Id = actualPath,
                    File = file
                };

            return new MockImportResolution() {
                Success = false
            };
        }
    }
}