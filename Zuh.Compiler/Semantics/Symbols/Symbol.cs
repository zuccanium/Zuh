using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public abstract record Symbol {
        public required string Name { get; init; }
    }
}