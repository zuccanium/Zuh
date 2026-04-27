using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public interface ITriviaHolder {
        public ImmutableArray<string>? TriviaLines { get; init; }
    }
}