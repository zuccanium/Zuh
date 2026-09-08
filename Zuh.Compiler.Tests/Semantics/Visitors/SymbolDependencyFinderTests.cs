using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Compilation;
using Zuh.Compiler.Semantics.Trackers.Unit;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Tests.Semantics.Visitors {
    public class SymbolDependencyFinderTests {
        [Fact]
        public void Visit_BasicDependency_Works() {
            var referencedExpressionDeclaration = new ExpressionDeclaration() {
                Name = new Label() {
                    Value = "referencedExpression"
                },
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = []
                    }
                }
            };
            
            var referencedExpressionDeclarationSymbol = new ExpressionDeclarationSymbol() {
                Name = "referencedExpression",
                ExpressionDeclaration = referencedExpressionDeclaration,
                UnitId = ""
            };

            var referencedExpressionDeclarationIdentifier = new Identifier() {
                Value = nameof(referencedExpressionDeclarationSymbol)
            };

            var referencingExpressionDeclaration = new ExpressionDeclaration() {
                Name = new Label() {
                    Value = "declaration"
                },
                Expression = new IdentifierExpression() {
                    Identifier = referencedExpressionDeclarationIdentifier
                }
            };

            var referencingExpressionDeclarationSymbol = new ExpressionDeclarationSymbol() {
                Name = "referencingExpression",
                ExpressionDeclaration = referencingExpressionDeclaration,
                UnitId = ""
            };

            var file = new ZuhFile() {
                RootStatements = [
                    referencedExpressionDeclaration,
                    referencingExpressionDeclaration
                ]
            };

            var unitSymbolTracker = new UnitSymbolTracker() {
                IdentifierToSymbol = {
                    [referencedExpressionDeclarationIdentifier] = referencedExpressionDeclarationSymbol
                },
                NodeToPersonalSymbol = {
                    [referencedExpressionDeclaration] = referencedExpressionDeclarationSymbol,
                    [referencingExpressionDeclaration] = referencingExpressionDeclarationSymbol
                }
            };

            var compilationSymbolTracker = new CompilationSymbolTracker();

            var visitor = new SymbolDependencyFinderVisitor() {
                UnitSymbolTracker = unitSymbolTracker,
                CompilationSymbolTracker = compilationSymbolTracker
            };
            
            visitor.Visit(file);

            var expectedDependencies = new Dictionary<Symbol, HashSet<Symbol>>() {
                [referencingExpressionDeclarationSymbol] = [
                    referencedExpressionDeclarationSymbol
                ]
            };
            
            Assert.Equivalent(expectedDependencies, compilationSymbolTracker.SymbolToDependencies);
        }
    }
}