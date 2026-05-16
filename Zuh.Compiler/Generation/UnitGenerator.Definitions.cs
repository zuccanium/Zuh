using Zuh.Compiler.Ast;
using Zuh.Compiler.Generation.Nodes;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Generation {
    public partial class UnitGenerator {
        private MappingNode schemaToMappingNode(Schema schema) {
            var node = new MappingNode();
            
            foreach(var entry in schema.Entries) {
                var sum = keyToSumNode(entry.Key);
                
                var valueNode = entry.Value is { } expressionValue
                    ? expressionToNode(expressionValue)
                    : new ScalarNode();

                foreach(var (key, value) in sum) {
                    node[key] = new MappingNode.Value() {
                        IsOptional = value.IsOptional,
                        Node = valueNode,
                        Documentation = (entry as IDocumentationHolder).FormattedLines
                    };
                }
            }

            return node;
        }

        private SumNode sumToSumNode(Sum sum) {
            var node = new SumNode();

            foreach(var entry in sum.Entries)
                foreach(var (key, value) in keyToSumNode(entry.Key)) {
                    node[key] = value;
                    node[key].Documentation = (entry as IDocumentationHolder).FormattedLines;
                }

            return node;
        }

        private SumNode keyToSumNode(Key key) {
            if(key is ExpressionKey expressionKey) {
                var node = expressionToNode(expressionKey.Expression);

                if(node is SumNode sumNode)
                    return sumNode;
            }

            if(key is StaticKey staticKey)
                return new SumNode() {
                    [staticKey.Name.Value] = new SumNode.Value() {
                        IsOptional = staticKey.IsOptional
                    }
                };

            throw new InvalidOperationException($"unknown {nameof(Key)} inheritor!!!");
        }
        
        private INode identifierToNode(Identifier identifier) {
            if(!Analyzer.SymbolTracker.Symbols.TryGetValue(identifier, out var symbol))
                throw new InvalidOperationException();

            return symbol switch {
                FunctionParameterSymbol functionParameterSymbol
                    => topStackFrame![functionParameterSymbol],
                
                ExpressionSymbol schemaSymbol
                    => expressionToNode(schemaSymbol.Expression),
                
                _ => throw new InvalidOperationException("unknown symbol type")
            };
        }
    }
}