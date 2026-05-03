using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Analyzers;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Tests.Semantics.Visitors {
    public class ScopeCreatorVisitorTests {
        // thank you olive for the random string of alphanumeric characters
        private const string ArbitraryName = "qapw340efoi8ujh";
        
        [Fact]
        public void ScopeCreatorVisitor_CreatesScope_WithZuhFile() {
            var file = new ZuhFile() {
                RootStatements = []
            };
            
            var scopeTracker = new ScopeTracker();

            var visitor = new ScopeCreatorVisitor() {
                ScopeTracker = scopeTracker
            };
            
            visitor.Visit(file);
            
            Assert.True(scopeTracker.NodeToPersonalScope.TryGetValue(file, out _));
        }

        [Fact]
        public void ScopeCreatorVisitor_CreatesScope_WithFunction() {
            var func = new FunctionDeclaration() {
                Name = new Label() {
                    Value = "func"
                },
                Function = new Function() {
                    Parameters = [],
                    Expression = new SchemaExpression() {
                        Schema = new Schema() {
                            Entries = []
                        }
                    }
                }
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    func
                ]
            };

            var scopeTracker = new ScopeTracker();

            var visitor = new ScopeCreatorVisitor() {
                ScopeTracker = scopeTracker
            };
            
            visitor.Visit(file);

            Assert.True(scopeTracker.NodeToPersonalScope.TryGetValue(func.Function, out _));
        }

        [Fact]
        public void ScopeCreatorVisitor_TracksIdentifier_WithZuhFile() {
            var identifier = new Identifier() {
                Value = ArbitraryName
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    new ExpressionDeclaration() {
                        Name = new Label() {
                            Value = "schema"
                        },
                        Expression = new SchemaExpression() {
                            Schema = new Schema() {
                                Entries = [
                                    new SchemaEntry() {
                                        Key = new SchemaEntryStaticKey() {
                                            Name = new Label() {
                                                Value = ""
                                            },
                                        },
                                        Value = new IdentifierExpression() {
                                            Identifier = identifier
                                        }
                                    }
                                ]
                            }
                        }
                    }
                ]
            };
            
            var scopeTracker = new ScopeTracker();

            var visitor = new ScopeCreatorVisitor() {
                ScopeTracker = scopeTracker
            };
            
            visitor.Visit(file);
            
            Assert.True(scopeTracker.NodeToPersonalScope.TryGetValue(file, out var fileScope));
            Assert.True(scopeTracker.NodeToEnclosingScope.TryGetValue(identifier, out var identifierEnclosingScope));
            
            Assert.Equal(identifierEnclosingScope, fileScope);
        }
        
        [Fact]
        public void ScopeCreatorVisitor_TracksIdentifier_WithFunction() {
            var identifier = new Identifier() {
                Value = ArbitraryName
            };

            var function = new Function() {
                Parameters = [],
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new SchemaEntryStaticKey() {
                                    Name = new Label() {
                                        Value = ""
                                    },
                                },
                                Value = new IdentifierExpression() {
                                    Identifier = identifier
                                }
                            }
                        ]
                    }
                }
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    new FunctionDeclaration() {
                        Name = new Label() {
                            Value = "function"
                        },
                        Function = function
                    }
                ]
            };
            
            var scopeTracker = new ScopeTracker();

            var visitor = new ScopeCreatorVisitor() {
                ScopeTracker = scopeTracker
            };
            
            visitor.Visit(file);
            
            Assert.True(scopeTracker.NodeToPersonalScope.TryGetValue(function, out var functionScope));
            Assert.True(scopeTracker.NodeToEnclosingScope.TryGetValue(identifier, out var identifierEnclosingScope));
            
            Assert.Equal(identifierEnclosingScope, functionScope);
        }
    }
}