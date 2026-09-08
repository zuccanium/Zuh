using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Compilation;
using Zuh.Compiler.Semantics.Trackers.Unit;

namespace Zuh.Compiler.Semantics.Visitors {
    public class SymbolDependencyFinderVisitor : Visitor {
        public required UnitSymbolTracker UnitSymbolTracker { get; init; }
        public required CompilationSymbolTracker CompilationSymbolTracker { get; init; }
        
        private HashSet<Symbol> symbolsInDeclaration = [];
        
        protected override List<Overload> Overloads
            => [
                // aggregates identifier symbols for the Declaration handler to use
                new Overload<Identifier>((node, next) => {
                    var symbol = UnitSymbolTracker.IdentifierToSymbol[node];

                    if(symbol is FunctionParameterSymbol)
                        return;
                    
                    symbolsInDeclaration.Add(symbol);
                }),
                new Overload<Declaration>((node, next) => {
                    next();
                    
                    var symbol = UnitSymbolTracker.NodeToPersonalSymbol[node];

                    CompilationSymbolTracker.SymbolToDependencies[symbol] = symbolsInDeclaration;
                    
                    symbolsInDeclaration = [];
                })
            ];
    }
}