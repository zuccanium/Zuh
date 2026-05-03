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
                
                FunctionInvocationExpression functionInvocationExpression
                    => functionInvocationExpressionToNode(functionInvocationExpression),
                
                _ => throw new InvalidOperationException("idk how this happened")
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
            var symbol = Analyzer.SymbolTracker.Symbols[expression.FunctionIdentifier];

            if(symbol is not FunctionSymbol functionSymbol)
                throw new InvalidOperationException("expected a function symbol");

            var function = functionSymbol.Function;
            
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
                
            if(leftNode is MappingNode leftMappingNode && rightNode is MappingNode rightMappingNode)
                return new MappingNode([
                    ..leftMappingNode,
                    ..rightMappingNode
                ]);
            
            if(leftNode is SumNode leftSumNode && rightNode is SumNode rightSumNode)
                return new SumNode([
                    ..leftSumNode,
                    ..rightSumNode
                ]);

            throw new InvalidOperationException();
        }
    }
}