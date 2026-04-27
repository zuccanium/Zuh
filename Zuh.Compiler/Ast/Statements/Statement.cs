using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public abstract record Statement : ZuhNode, ITriviaHolder {
        public ImmutableArray<string>? TriviaLines { get; init; }
    }
}