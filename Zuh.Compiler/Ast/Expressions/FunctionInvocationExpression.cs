using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record FunctionInvocationExpression : Expression {
        public required Identifier FunctionIdentifier { get; init; }
        public required ImmutableArray<Expression> Arguments { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return FunctionIdentifier;
            
            foreach(var argument in Arguments)
                yield return argument;
        }
    }
}