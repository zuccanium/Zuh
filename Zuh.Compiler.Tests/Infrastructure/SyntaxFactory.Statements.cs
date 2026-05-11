using Zuh.Compiler.Ast;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public static ImportStatement ImportStatementPlaceholder
            => new() { Module = StringLiteralPlaceholder };

        public static Statement StatementPlaceholder
            => DeclarationPlaceholder;
        
        public static MappingNode CreateImportStatement(out Func<ImportStatement> getter, ImportStatement value) {
            var stringLiteralNode = CreateStringLiteral(out var stringLiteralGetter, value.Module);
            var node = Mark(out var importStatementMarker, $"import {stringLiteralNode};");

            getter = () => new ImportStatement() {
                Module = stringLiteralGetter(),
                SourceSpan = importStatementMarker.SourceSpan
            };

            return node;
        }
        
        public static MappingNode CreateImportStatement(out Func<ImportStatement> getter)
            => CreateImportStatement(out getter, ImportStatementPlaceholder);

        public static MappingNode CreateStatement(out Func<Statement> getter, Statement value) {
            (MappingNode node, Func<Statement> getter) createStatementTuple<TNode>(
                NodeCreator<TNode> creator,
                TNode value
            ) where TNode : Statement
                => createTuple<Statement, TNode>(creator, value);

            var nodeAndGetterTuple = value switch {
                Declaration declarationValue
                    => createStatementTuple(CreateDeclaration, declarationValue),

                ImportStatement importStatementValue
                    => createStatementTuple(CreateImportStatement, importStatementValue),

                _ => throw new NotImplementedException()
            };

            getter = nodeAndGetterTuple.getter;

            return nodeAndGetterTuple.node;
        }

        public static MappingNode CreateStatement(out Func<Statement> getter)
            => CreateStatement(out getter, StatementPlaceholder);
    }
}