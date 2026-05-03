using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public abstract record Symbol {
        public required string Name { get; init; }
        public required SymbolVisibility Visibility { get; init; }

        public enum SymbolVisibility {
            Local,
            Exported
        }
    }
}