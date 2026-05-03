using System.Diagnostics;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Generation.Nodes;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Analyzers;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Generation {
    public class UnitGenerator {
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
                        => new KeysNode([new KeysNode.Value() {
                            Key = staticKey.Name.Value,
                            IsOptional = entry.Key.IsOptional
                        }]),

                    SchemaEntryExpressionKey expressionKey
                        => expressionToNode(expressionKey.Expression) is KeysNode keysNode
                            ? keysNode
                            : throw new InvalidOperationException(),
                    
                    _ => throw new InvalidOperationException()
                };
                
                var valueNode = entry.Value is { } expressionValue
                    ? expressionToNode(expressionValue)
                    : new ScalarNode();

                foreach(var key in keys)
                    node[key.Key] = new MappingNode.Value() {
                        IsOptional = key.IsOptional,
                        Node = valueNode
                    };
            }

            return node;
        }

        private KeysNode keysToKeysNode(Keys keys) {
            var node = new KeysNode();
            
            foreach(var entry in keys.Entries)
                node.Add(new KeysNode.Value() {
                    Key = entry.Name.Value
                });

            return node;
        }
        
        private INode identifierToNode(Identifier identifier) {
            if(!Analyzer.SymbolTracker.Symbols.TryGetValue(identifier, out var symbol))
                throw new InvalidOperationException();

            if(symbol is FunctionParameterSymbol functionParameterSymbol)
                return topStackFrame![functionParameterSymbol];
            
            if(symbol is SchemaSymbol schemaSymbol)
                return schemaToMappingNode(schemaSymbol.Schema);

            if (symbol is KeysSymbol keysSymbol)
                return keysToKeysNode(keysSymbol.Keys);

            throw new InvalidOperationException();
        }
        
        private INode expressionToNode(Expression expression) {
            if(expression is IdentifierExpression identifierExpression)
                return identifierExpressionToNode(identifierExpression);
            
            if(expression is SchemaExpression schemaExpression)
                return schemaExpressionToNode(schemaExpression);

            if(expression is KeysExpression keysExpression)
                return keysExpressionToNode(keysExpression);
            
            if(expression is ArrayExpression arrayExpression)
                return arrayExpressionToNode(arrayExpression);
            
            if(expression is IntersectionExpression intersectionExpression)
                return intersectionExpressionToNode(intersectionExpression);

            if(expression is FunctionInvocationExpression functionInvocationExpression)
                return functionInvocationExpressionToNode(functionInvocationExpression);

            throw new InvalidOperationException("idk how this happened");
        }
        
        private INode identifierExpressionToNode(IdentifierExpression expression)
            => identifierToNode(expression.Identifier);

        private INode schemaExpressionToNode(SchemaExpression expression)
            => schemaToMappingNode(expression.Schema);
        
        private INode keysExpressionToNode(KeysExpression expression)
            => keysToKeysNode(expression.Keys);

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
            
            if(leftNode is KeysNode leftKeysNode && rightNode is KeysNode rightKeysNode)
                return new KeysNode([
                    ..leftKeysNode,
                    ..rightKeysNode
                ]);

            throw new InvalidOperationException();
        }
    }
}