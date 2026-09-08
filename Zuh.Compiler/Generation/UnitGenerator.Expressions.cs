using Zuh.Compiler.Ast;
using Zuh.Compiler.Generation.Nodes;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Generation {
    public partial class UnitGenerator {
        private INode expressionToNode(Expression expression)
            => expression switch {
                IdentifierExpression identifierExpression
                    => identifierExpressionToNode(identifierExpression),
                
                SchemaExpression schemaExpression
                    => schemaExpressionToNode(schemaExpression),
                
                SumExpression sumExpression
                    => sumExpressionToNode(sumExpression),
                
                ArrayExpression arrayExpression
                    => arrayExpressionToNode(arrayExpression),
                
                IntersectionExpression intersectionExpression
                    => intersectionExpressionToNode(intersectionExpression),
                
                UnionExpression unionExpression
                    => unionExpressionToNode(unionExpression),
                
                FunctionInvocationExpression functionInvocationExpression
                    => functionInvocationExpressionToNode(functionInvocationExpression),
                
                _ => throw new InvalidOperationException($"unknown {nameof(Expression)} inheritor!!!")
            };

        private INode identifierExpressionToNode(IdentifierExpression expression)
            => identifierToNode(expression.Identifier);

        private INode schemaExpressionToNode(SchemaExpression expression)
            => schemaToMappingNode(expression.Schema);
        
        private INode sumExpressionToNode(SumExpression expression)
            => sumToSumNode(expression.Sum);

        private INode arrayExpressionToNode(ArrayExpression expression)
            => new ArrayNode() {
                Node = expressionToNode(expression.Expression)
            };
        
        private INode functionInvocationExpressionToNode(FunctionInvocationExpression expression) {
            var symbol = Analyzer.UnitSymbolTracker.IdentifierToSymbol[expression.FunctionIdentifier];

            if(symbol is not FunctionDeclarationSymbol functionSymbol)
                throw new InvalidOperationException("expected a function symbol");

            var function = functionSymbol.FunctionDeclaration.Function;
            
            if(expression.Arguments.Length != function.Parameters.Length)
                throw new InvalidOperationException("parameter length mismatch");

            var newStackFrame = new Dictionary<Symbol, INode>();
            
            foreach(var (parameter, argument) in functionSymbol.Parameters.Zip(expression.Arguments))
                newStackFrame[parameter] = expressionToNode(argument);

            stackFrames.Push(newStackFrame);
            
            var node = expressionToNode(function.Expression);

            stackFrames.Pop();

            return node;
        }
        
        private INode intersectionExpressionToNode(IntersectionExpression expression) {
            var leftNode = expressionToNode(expression.Left);
            var rightNode = expressionToNode(expression.Right);

            if(leftNode is MappingNode leftMappingNode && rightNode is MappingNode rightMappingNode) {
                var node = new MappingNode();

                foreach(var (key, leftValue) in leftMappingNode)
                    if(rightMappingNode.TryGetValue(key, out var rightValue))
                        node[key] = new MappingNode.Value() {
                            IsOptional = leftValue.IsOptional || rightValue.IsOptional,
                        
                            // pray that the semantic analyzer handled the comparison earlier
                            Node = leftValue.Node
                        };

                return node;
            }

            if(leftNode is SumNode leftSumNode && rightNode is SumNode rightSumNode) {
                var node = new SumNode();
                
                foreach(var (key, leftValue) in leftSumNode)
                    if(rightSumNode.TryGetValue(key, out var rightValue))
                        node[key] = new SumNode.Value() {
                            IsOptional = leftValue.IsOptional || rightValue.IsOptional,
                        };

                return node;
            }

            throw new InvalidOperationException("unknown intersection expression types");
        }
        
        private INode unionExpressionToNode(UnionExpression expression) {
            var leftNode = expressionToNode(expression.Left);
            var rightNode = expressionToNode(expression.Right);

            if(leftNode is MappingNode leftMappingNode && rightNode is MappingNode rightMappingNode) {
                var node = new MappingNode([..leftMappingNode]);

                foreach(var (key, newValue) in rightMappingNode)
                    node[key] = new MappingNode.Value() {
                        // still praying
                        Node = newValue.Node,
                        
                        IsOptional = node.TryGetValue(key, out var oldValue)
                            ? oldValue.IsOptional && newValue.IsOptional
                            : newValue.IsOptional
                    };

                return node;
            }

            if(leftNode is SumNode leftSumNode && rightNode is SumNode rightSumNode) {
                var node = new SumNode([..leftSumNode]);

                foreach(var (key, newValue) in rightSumNode)
                    node[key] = new SumNode.Value() {
                        IsOptional = node.TryGetValue(key, out var oldValue)
                            ? oldValue.IsOptional && newValue.IsOptional
                            : newValue.IsOptional
                    };

                return node;
            }

            throw new InvalidOperationException("unknown union expression types");
        }
    }
}