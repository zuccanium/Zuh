using Zuh.Compiler.Ast;
using Zuh.Compiler.Generation.Nodes;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Generation {
    public partial class UnitGenerator {
        private MappingNode schemaToMappingNode(Schema schema) {
            var node = new MappingNode();
            
            foreach(var entry in schema.Entries) {
                var keys = entry.Key switch {
                    StaticKey staticKey
                        => new SumNode([new SumNode.Value() {
                            Key = staticKey.Name.Value,
                            IsOptional = entry.Key.IsOptional
                        }]),

                    ExpressionKey expressionKey
                        => expressionToNode(expressionKey.Expression) is SumNode sumNode
                            ? sumNode
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

        private SumNode sumToSumNode(Sum sum) {
            var node = new SumNode();

            foreach(var entry in sum.Entries)
                node.AddRange(keyToSumNode(entry.Key));

            return node;
        }

        private SumNode keyToSumNode(Key key) {
            if(key is ExpressionKey expressionKey) {
                var node = expressionToNode(expressionKey.Expression);

                if(node is SumNode sumNode)
                    return sumNode;
            }

            if(key is StaticKey staticKey)
                return [
                    new SumNode.Value() {
                        Key = staticKey.Name.Value,
                        IsOptional = staticKey.IsOptional
                    }
                ];

            throw new InvalidOperationException();
        }
        
        private INode identifierToNode(Identifier identifier) {
            if(!Analyzer.SymbolTracker.Symbols.TryGetValue(identifier, out var symbol))
                throw new InvalidOperationException();

            return symbol switch {
                FunctionParameterSymbol functionParameterSymbol
                    => topStackFrame![functionParameterSymbol],
                
                ExpressionSymbol schemaSymbol
                    => expressionToNode(schemaSymbol.Expression),
                
                _ => throw new InvalidOperationException()
            };
        }
    }
}