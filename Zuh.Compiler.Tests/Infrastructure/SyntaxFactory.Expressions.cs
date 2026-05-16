using Zuh.Compiler.Ast;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public static IdentifierExpression IdentifierExpressionPlaceholder
            => new() { Identifier = IdentifierPlaceholder };
        
        public static SchemaExpression SchemaExpressionPlaceholder
            => new() { Schema = SchemaPlaceholder };
        
        public static SumExpression SumExpressionPlaceholder
            => new() { Sum = SumPlaceholder };

        
        public static ParenthesizedExpression ParenthesizedExpressionPlaceholder
            => new() { Expression = ExpressionPlaceholder };
        
        public static ArrayExpression ArrayExpressionPlaceholder
            => new() { Expression = ExpressionPlaceholder };
        
        public static FunctionInvocationExpression FunctionInvocationExpression
            => new() {
                FunctionIdentifier = IdentifierPlaceholder,
                Arguments = []
            };

        public static IntersectionExpression IntersectionExpressionPlaceholder
            => new() {
                Left = ExpressionPlaceholder,
                Right = ExpressionPlaceholder
            };
        
        
        public static UnionExpression UnionExpressionPlaceholder
            => new() {
                Left = ExpressionPlaceholder,
                Right = ExpressionPlaceholder
            };

        
        public static Expression ExpressionPlaceholder
            => IdentifierExpressionPlaceholder;
        
        
        private static MappingNode createDefinitionExpression<TDefinitionExpression, TDefinition>(
            out Func<TDefinitionExpression> getter,
            TDefinitionExpression value,
            NodeCreator<TDefinition> creator,
            Func<TDefinitionExpression, TDefinition> innerGetter,
            Func<TDefinition, TDefinitionExpression> outerGetter
        ) where TDefinition : ZuhNode where TDefinitionExpression : ZuhNode {
            var node = Mark(out var definitionExpressionMarker, $"{creator(out var definitionGetter, innerGetter(value))}");

            getter = () => outerGetter(definitionGetter()) with {
                SourceSpan = definitionExpressionMarker.SourceSpan
            };

            return node;
        }
        
        public static MappingNode CreateIdentifierExpression(out Func<IdentifierExpression> getter, IdentifierExpression value)
            => createDefinitionExpression(
                out getter,
                value,
                CreateIdentifier,
                identifierExpression => identifierExpression.Identifier,
                definition => new IdentifierExpression() {
                    Identifier = definition
                }
            );
        
        public static MappingNode CreateSchemaExpression(out Func<SchemaExpression> getter, SchemaExpression value)
            => createDefinitionExpression(
                out getter,
                value,
                CreateSchema,
                schemaExpression => schemaExpression.Schema,
                definition => new SchemaExpression() {
                    Schema = definition
                }
            );
        
        public static MappingNode CreateSumExpression(out Func<SumExpression> getter, SumExpression value)
            => createDefinitionExpression(
                out getter,
                value,
                CreateSum,
                sumExpression => sumExpression.Sum,
                definition => new SumExpression() {
                    Sum = definition
                }
            );

        private static MappingNode createBinaryExpression<TBinaryExpression>(
            out Func<TBinaryExpression> getter,
            TBinaryExpression value,
            string symbol,
            Func<Expression, Expression, TBinaryExpression> creator
        ) where TBinaryExpression : BinaryExpression {
            var left = CreateExpression(out var leftGetter, value.Left);
            var right = CreateExpression(out var rightGetter, value.Right);
            
            var node = Mark(out var intersectionExpressionMarker, $"{left} {symbol} {right}");

            getter = () => creator(leftGetter(), rightGetter()) with {
                SourceSpan = intersectionExpressionMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateIntersectionExpression(out Func<IntersectionExpression> getter, IntersectionExpression value)
            => createBinaryExpression(
                out getter,
                value,
                "&",
                (left, right) => new IntersectionExpression() {
                    Left = left,
                    Right = right
                }
            );

        public static MappingNode CreateUnionExpression(out Func<UnionExpression> getter, UnionExpression value)
            => createBinaryExpression(
                out getter,
                value,
                "|",
                (left, right) => new UnionExpression() {
                    Left = left,
                    Right = right
                }
            );

        public static MappingNode CreateFunctionInvocationExpression(
            out Func<FunctionInvocationExpression> getter,
            FunctionInvocationExpression value
        ) {
            var argumentNodes = value.Arguments
                .SelectWithOut(
                    out var argumentGetters,
                    (Expression source, out Func<Expression> outValue)
                        => CreateExpression(out outValue, source)
                );

            var identifierNode = CreateIdentifier(out var identifierGetter, value.FunctionIdentifier);
            var node = Mark(out var functionInvocationExpressionMarker, $"{identifierNode}({argumentNodes.MarkAsJoined(", ")})");

            getter = () => new FunctionInvocationExpression() {
                FunctionIdentifier = identifierGetter(),
                Arguments = [
                    ..argumentGetters
                        .Select(getter => getter())
                ],
                SourceSpan = functionInvocationExpressionMarker.SourceSpan
            };

            return node;
        }
        
        private static MappingNode createUnaryExpression<TUnaryExpression>(
            out Func<TUnaryExpression> getter,
            TUnaryExpression value,
            Func<MappingNode, SpanMarkingInterpolatedStringHandler> stringGetter,
            Func<Expression, TUnaryExpression> creator
        ) where TUnaryExpression : UnaryExpression {
            var expression = CreateExpression(out var expressionGetter, value.Expression);
            var node = Mark(out var expressionMarker, stringGetter(expression));

            getter = () => creator(expressionGetter()) with {
                SourceSpan = expressionMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateParenthesizedExpression(out Func<ParenthesizedExpression> getter, ParenthesizedExpression value)
            => createUnaryExpression(
                out getter,
                value,
                node => $"({node})",
                expression => new ParenthesizedExpression() {
                    Expression = expression
                }
            );
        
        public static MappingNode CreateArrayExpression(out Func<ArrayExpression> getter, ArrayExpression value)
            => createUnaryExpression(
                out getter,
                value,
                node => $"{node}[]",
                expression => new ArrayExpression() {
                    Expression = expression
                }
            );
        
        public static MappingNode CreateExpression(out Func<Expression> getter, Expression value) {
            (MappingNode node, Func<Expression> getter) createExpressionTuple<TNode>(
                NodeCreator<TNode> creator,
                TNode value
            ) where TNode : Expression
                => createTuple<Expression, TNode>(creator, value);
            
            var nodeAndGetterTuple = value switch {
                IdentifierExpression identifierExpressionValue
                    => createExpressionTuple(CreateIdentifierExpression, identifierExpressionValue),
                
                SchemaExpression schemaExpressionValue
                    => createExpressionTuple(CreateSchemaExpression, schemaExpressionValue),
                    
                SumExpression sumExpressionValue
                    => createExpressionTuple(CreateSumExpression, sumExpressionValue),
                
                ParenthesizedExpression parenthesizedExpressionValue
                    => createExpressionTuple(CreateParenthesizedExpression, parenthesizedExpressionValue),
                
                ArrayExpression arrayExpressionValue
                    => createExpressionTuple(CreateArrayExpression, arrayExpressionValue),
                
                FunctionInvocationExpression functionInvocationExpressionValue
                    => createExpressionTuple(CreateFunctionInvocationExpression, functionInvocationExpressionValue),
                    
                IntersectionExpression intersectionExpressionValue
                    => createExpressionTuple(CreateIntersectionExpression, intersectionExpressionValue),
                
                UnionExpression unionExpressionValue
                    => createExpressionTuple(CreateUnionExpression, unionExpressionValue),
                
                _ => throw new NotImplementedException()
            };

            getter = nodeAndGetterTuple.getter;
            
            return nodeAndGetterTuple.node;
        }

        public static MappingNode CreateExpression(out Func<Expression> getter)
            => CreateExpression(out getter, ExpressionPlaceholder);
    }
}