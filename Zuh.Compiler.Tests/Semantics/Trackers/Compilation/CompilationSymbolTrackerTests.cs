using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Compilation;

namespace Zuh.Compiler.Tests.Semantics.Trackers.Compilation {
    public class CompilationSymbolTrackerTests {
        [Fact]
        public void ResolveCircularDependencies_NoCircularDependencies_DoesntCreateAny() {
            var symbol1 = new ExpressionDeclarationSymbol() {
                Name = "symbol1",
                ExpressionDeclaration = null!,
                UnitId = ""
            };
            
            var symbol2 = new ExpressionDeclarationSymbol() {
                Name = "symbol2",
                ExpressionDeclaration = null!,
                UnitId = ""
            };

            var compilationSymbolTracker = new CompilationSymbolTracker() {
                SymbolToDependencies = {
                    [symbol1] = [
                        symbol2
                    ],
                    [symbol2] = []
                }
            };
            
            compilationSymbolTracker.ResolveCircularDependencies();
            
            Assert.Empty(compilationSymbolTracker.SymbolToCircularDependencies);
        }
        
        [Fact]
        public void ResolveCircularDependencies_OneCircularDependency_CreatesOne() {
            var symbol1 = new ExpressionDeclarationSymbol() {
                Name = "symbol1",
                ExpressionDeclaration = null!,
                UnitId = ""
            };
            
            var symbol2 = new ExpressionDeclarationSymbol() {
                Name = "symbol2",
                ExpressionDeclaration = null!,
                UnitId = ""
            };

            var compilationSymbolTracker = new CompilationSymbolTracker() {
                SymbolToDependencies = {
                    [symbol1] = [
                        symbol2
                    ],
                    [symbol2] = [
                        symbol1
                    ]
                }
            };
            
            compilationSymbolTracker.ResolveCircularDependencies();
            
            var expectedCircularDependencies = new Dictionary<Symbol, HashSet<CompilationSymbolTracker.CircularDependency>>() {
                [symbol1] = [
                    new CompilationSymbolTracker.CircularDependency(symbol1, symbol2, symbol1)
                ],
                [symbol2] = [
                    new CompilationSymbolTracker.CircularDependency(symbol2, symbol1, symbol2)
                ]
            };
            
            Assert.Equivalent(expectedCircularDependencies, compilationSymbolTracker.SymbolToCircularDependencies);
        }
        
        [Fact]
        public void ResolveCircularDependencies_TwoCircularDependencies_CreatesTwo() {
            var symbol1 = new ExpressionDeclarationSymbol() {
                Name = "symbol1",
                ExpressionDeclaration = null!,
                UnitId = ""
            };
            
            var symbol2 = new ExpressionDeclarationSymbol() {
                Name = "symbol2",
                ExpressionDeclaration = null!,
                UnitId = ""
            };
            
            var symbol3 = new ExpressionDeclarationSymbol() {
                Name = "symbol3",
                ExpressionDeclaration = null!,
                UnitId = ""
            };

            var compilationSymbolTracker = new CompilationSymbolTracker() {
                SymbolToDependencies = {
                    [symbol1] = [
                        symbol2,
                        symbol3
                    ],
                    [symbol2] = [
                        symbol1
                    ],
                    [symbol3] = [
                        symbol1
                    ]
                }
            };
            
            compilationSymbolTracker.ResolveCircularDependencies();
            
            var expectedCircularDependencies = new Dictionary<Symbol, HashSet<CompilationSymbolTracker.CircularDependency>>() {
                [symbol1] = [
                    new CompilationSymbolTracker.CircularDependency(symbol1, symbol2, symbol1),
                    new CompilationSymbolTracker.CircularDependency(symbol1, symbol3, symbol1)
                ],
                [symbol2] = [
                    new CompilationSymbolTracker.CircularDependency(symbol2, symbol1, symbol2)
                ],
                [symbol3] = [
                    new CompilationSymbolTracker.CircularDependency(symbol3, symbol1, symbol3)
                ]
            };
            
            Assert.Equivalent(expectedCircularDependencies, compilationSymbolTracker.SymbolToCircularDependencies);
        }
    }
}