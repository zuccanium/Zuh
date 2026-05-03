using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Semantics;

namespace Zuh.Compiler.Tests.Semantics {
    public class MockImportHandler : IImportHandler {
        public Dictionary<string, ZuhFile> Files { get; private init; } = [];

        public ImportResolution ResolveModule(string sourceId, string module) {
            var actualPath = Path.Join(Path.GetDirectoryName(sourceId), module);

            if(Files.ContainsKey(actualPath))
                return new ImportResolution() {
                    Success = true,
                    Id = actualPath
                };

            return new ImportResolution() {
                Success = false
            };
        }

        public ZuhFile FetchContent(ImportResolution resolution)
            => Files[resolution.Id!];
    }
}