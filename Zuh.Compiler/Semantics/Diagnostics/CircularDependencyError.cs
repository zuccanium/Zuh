using System.Collections.Immutable;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Trackers.Compilation;

namespace Zuh.Compiler.Semantics.Diagnostics {
    public record CircularDependencyError : Error {
        public required string Name { get; init; }
        public required CompilationSymbolTracker.CircularDependency CircularDependency { get; init; }

        public override string Message
            => $"{Name} creates a circular dependency of {CircularDependency}";
    }
}