using System.Diagnostics;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Emission.Nodes;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Analyzers;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Emission {
    public class UnitGenerator {
        private record struct KeyInfo(string Name, bool IsOptional);

        private Stack<Dictionary<Symbol, INode>> stackFrames = [];
        
        public required UnitAnalyzer Analyzer { get; init; }
        public MappingNode Root { get; private init; } = new();

        private Dictionary<Symbol, INode>? topStackFrame
            => stackFrames.TryPeek(out var top)
                ? top
                : null;
        
        public void Generate() {
            foreach(var statement in Analyzer.File.RootStatements) {
                if(statement is not SchemaDeclaration { IsExport: true } schemaDeclaration)
                    continue;
                
                generateSchemaDeclaration(schemaDeclaration);
            }
        }

        private void generateSchemaDeclaration(SchemaDeclaration schemaDeclaration) {
            Root[schemaDeclaration.Name.Value] = new MappingNode.Value() {
                Node = schemaToMappingNode(schemaDeclaration.Schema)
            };
        }
        
        private MappingNode schemaToMappingNode(Schema schema) {
            var node = new MappingNode();
            
            foreach(var entry in schema.Entries) {
                var keys = entry.Key switch {
                    SchemaEntryStaticKey staticKey
                        => (KeyInfo[])[new KeyInfo(staticKey.Name.Value, staticKey.IsOptional)],

                    _ => throw new InvalidOperationException("qua")
                };
                
                var valueNode = entry.Value is { } expressionValue
                    ? expressionToNode(expressionValue)
                    : new ScalarNode();

                foreach(var key in keys)
                    node[key.Name] = new MappingNode.Value() {
                        IsOptional = key.IsOptional,
                        Node = valueNode
                    };
            }

            return node;
        }
        
        private INode identifierToNode(Identifier identifier) {
            if(!Analyzer.SymbolTracker.Symbols.TryGetValue(identifier, out var symbol))
                throw new InvalidOperationException();

            if(symbol is FunctionParameterSymbol functionParameterSymbol)
                return topStackFrame![functionParameterSymbol];
            
            if(symbol is SchemaSymbol schemaSymbol)
                return schemaToMappingNode(schemaSymbol.Schema);

            if(symbol is KeysSymbol keysSymbol)
                throw new NotImplementedException();

            throw new InvalidOperationException();
        }
        
        private INode expressionToNode(Expression expression) {
            if(expression is SchemaExpression schemaExpression)
                return schemaExpressionToNode(schemaExpression);

            if(expression is ArrayExpression arrayExpression)
                return arrayExpressionToNode(arrayExpression);
            
            if(expression is IntersectionExpression intersectionExpression)
                return intersectionExpressionToNode(intersectionExpression);

            if(expression is FunctionInvocationExpression functionInvocationExpression)
                return functionInvocationExpressionToNode(functionInvocationExpression);

            throw new InvalidOperationException("idk how this happened");
        }

        private INode schemaExpressionToNode(SchemaExpression expression)
            => schemaToMappingNode(expression.Schema);

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

        private INode identifierExpressionToNode(IdentifierExpression expression)
            => identifierToNode(expression.Identifier);
        
        private INode intersectionExpressionToNode(IntersectionExpression expression) {
            var leftNode = expressionToNode(expression.Left);
            var rightNode = expressionToNode(expression.Right);
                
            if(leftNode is not MappingNode leftMappingNode || rightNode is not MappingNode rightMappingNode)
                throw new InvalidOperationException();

            return new MappingNode([
                ..leftMappingNode,
                ..rightMappingNode
            ]);
        }
    }
}