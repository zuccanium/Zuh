using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics;

namespace Zuh.Compiler.Tests.Semantics {
    public class MockImportResolution : IImportResolution {
        public bool Success { get; init; }
        
        public string? Id { get; init; }
        
        public ZuhFile? File { get; init; }

        public ZuhFile FetchFile(out DiagnosticCollector diagnostics) {
            diagnostics = [];

            return File!;
        }
    }
}