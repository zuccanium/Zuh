using Zuh.Compiler.Ast;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public static ExpressionDeclaration ExpressionDeclarationPlaceholder
            => new() {
                Name = LabelPlaceholder,
                Expression = ExpressionPlaceholder
            };
        
        public static FunctionDeclaration FunctionDeclarationPlaceholder
            => new() {
                Name = LabelPlaceholder,
                Function = FunctionPlaceholder
            };

        public static Declaration DeclarationPlaceholder
            => ExpressionDeclarationPlaceholder;

        private static MappingNode createDefinitionDeclaration<TDeclaration, TDefinition>(
            out Func<TDeclaration> getter,
            TDeclaration value,
            NodeCreator<TDefinition> creator,
            Func<TDeclaration, TDefinition> innerGetter,
            Func<TDefinition, Label, TDeclaration> outerGetter
        ) where TDeclaration : Declaration where TDefinition : ZuhNode {
            var labelNode = CreateLabel(out var labelGetter, value.Name);
            var definitionNode = creator(out var definitionGetter, innerGetter(value));

            var node = value.IsExport
                ? Mark(out var definitionDeclarationMarker, $"export {labelNode} {definitionNode};")
                : Mark(out definitionDeclarationMarker, $"{labelNode} {definitionNode};");

            getter = () => outerGetter(definitionGetter(), labelGetter()) with {
                IsExport = value.IsExport,
                DocumentationLines = [],
                SourceSpan = definitionDeclarationMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateExpressionDeclaration(out Func<ExpressionDeclaration> getter, ExpressionDeclaration value)
            => createDefinitionDeclaration(
                out getter,
                value,
                CreateExpression,
                (declaration) => declaration.Expression,
                (expression, label) => new ExpressionDeclaration() {
                    Name = label,
                    Expression = expression
                }
            );
        
        public static MappingNode CreateExpressionDeclaration(out Func<ExpressionDeclaration> getter)
            => CreateExpressionDeclaration(out getter, ExpressionDeclarationPlaceholder);
        
        public static MappingNode CreateFunctionDeclaration(out Func<FunctionDeclaration> getter, FunctionDeclaration value)
            => createDefinitionDeclaration(
                out getter,
                value,
                CreateFunction,
                (declaration) => declaration.Function,
                (expression, label) => new FunctionDeclaration() {
                    Name = label,
                    Function = expression
                }
            );
        
        public static MappingNode CreateFunctionDeclaration(out Func<FunctionDeclaration> getter)
            => CreateFunctionDeclaration(out getter, FunctionDeclarationPlaceholder);

        public static MappingNode CreateDeclaration(out Func<Declaration> getter, Declaration value) {
            (MappingNode node, Func<Declaration> getter) createDeclarationTuple<TNode>(
                NodeCreator<TNode> creator,
                TNode value
            ) where TNode : Declaration
                => createTuple<Declaration, TNode>(creator, value);

            var nodeAndGetterTuple = value switch {
                ExpressionDeclaration expressionDeclarationValue
                    => createDeclarationTuple(CreateExpressionDeclaration, expressionDeclarationValue),

                FunctionDeclaration functionDeclarationValue
                    => createDeclarationTuple(CreateFunctionDeclaration, functionDeclarationValue),

                _ => throw new NotImplementedException()
            };
            
            getter = nodeAndGetterTuple.getter;

            return nodeAndGetterTuple.node;
        }

        public static MappingNode CreateDeclaration(out Func<Declaration> getter)
            => CreateDeclaration(out getter, DeclarationPlaceholder);
    }
}