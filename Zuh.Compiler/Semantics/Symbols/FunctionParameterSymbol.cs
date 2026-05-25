using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public class FunctionParameterSymbol : Symbol {
        public required FunctionParameter FunctionParameter { get; init; }
        
        public override ZuhNode Node
            => FunctionParameter;
    }
}