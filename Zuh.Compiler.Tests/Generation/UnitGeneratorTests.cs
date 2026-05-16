using Zuh.Compiler.Ast;
using Zuh.Compiler.Generation;
using Zuh.Compiler.Generation.Nodes;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Analyzers;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Tests.Generation {
    public class UnitGeneratorTests {
        public class SingleSchema {
            public abstract class TestSet {
                protected abstract Schema Schema { get; }
                protected abstract INode SchemaNode { get; }
                
                [Fact]
                public void Works() {
                    var schema = new ExpressionDeclaration() {
                        IsExport = true,
                        Name = new Label() {
                            Value = "schema"
                        },
                        Expression = new SchemaExpression() {
                            Schema = Schema
                        }
                    };

                    var file = new ZuhFile() {
                        RootStatements = [
                            schema
                        ]
                    };

                    var fileScope = new Scope();

                    var scopeTracker = new ScopeTracker() {
                        NodeToPersonalScope = {
                            [file] = fileScope
                        },
                        NodeToEnclosingScope = {
                            [schema] = fileScope
                        }
                    };

                    var symbolTracker = new SymbolTracker();

                    var analyzer = new UnitAnalyzer() {
                        CompilationAnalyzer = null!,
                        File = file,
                        UnitId = "main.zuh",
                        ScopeTracker = scopeTracker,
                        SymbolTracker = symbolTracker,
                    };

                    var generator = new UnitGenerator() {
                        Analyzer = analyzer
                    };
                    
                    var root = generator.Generate();
                    
                    Assert.Equivalent(new MappingNode() {
                        [nameof(schema)] = new MappingNode.Value() {
                            Node = SchemaNode
                        }
                    }, root);
                }
            }

            public class SingleScalar : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = "key"
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["key"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        }
                    };
            }
            
            public class ScalarAndSubSchema : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = "schemaKey"
                                    }
                                },
                                Value = new SchemaExpression() {
                                    Schema = new Schema() {
                                        Entries = [
                                            new SchemaEntry() {
                                                Key = new StaticKey() {
                                                    Name = new Label() {
                                                        Value = "a"
                                                    }
                                                }
                                            },
                                            new SchemaEntry() {
                                                Key = new StaticKey() {
                                                    Name = new Label() {
                                                        Value = "b"
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["schemaKey"] = new MappingNode.Value() {
                            Node = new MappingNode() {
                                ["a"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                },
                                ["b"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                }
                            }
                        }
                    };
            }
            
            public class OptionalScalar : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    IsOptional = true,
                                    Name = new Label() {
                                        Value = "key"
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["key"] = new MappingNode.Value() {
                            IsOptional = true,
                            Node = new ScalarNode()
                        }
                    };
            }
            
            public class SingleSumKey : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new ExpressionKey() {
                                    Expression = new SumExpression() {
                                        Sum = new Sum() {
                                            Entries = [
                                                new SumEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "a"
                                                        }
                                                    }
                                                }
                                            ]
                                        }
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["a"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        }
                    };
            }
            
            public class MultipleSumKey : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new ExpressionKey() {
                                    Expression = new SumExpression() {
                                        Sum = new Sum() {
                                            Entries = [
                                                new SumEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "a"
                                                        }
                                                    }
                                                },
                                                new SumEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "b"
                                                        }
                                                    }
                                                },
                                                new SumEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "c"
                                                        }
                                                    }
                                                }
                                            ]
                                        }
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["a"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        },
                        ["b"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        },
                        ["c"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        },
                    };
            }
            
            public class UnionSumKey : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new ExpressionKey() {
                                    Expression = new UnionExpression() {
                                        Left = new SumExpression() {
                                            Sum = new Sum() {
                                                Entries = [
                                                    new SumEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = "a"
                                                            }
                                                        }
                                                    },
                                                    new SumEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = "b"
                                                            }
                                                        }
                                                    }
                                                ]
                                            }
                                        },
                                        Right = new SumExpression() {
                                            Sum = new Sum() {
                                                Entries = [
                                                    new SumEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = "c"
                                                            }
                                                        }
                                                    }
                                                ]
                                            }
                                        }
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["a"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        },
                        ["b"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        },
                        ["c"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        },
                    };
            }
            
            public class IntersectionSumKey : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new ExpressionKey() {
                                    Expression = new UnionExpression() {
                                        Left = new SumExpression() {
                                            Sum = new Sum() {
                                                Entries = [
                                                    new SumEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = "a"
                                                            }
                                                        }
                                                    },
                                                    new SumEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = "b"
                                                            }
                                                        }
                                                    },
                                                    new SumEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = "c"
                                                            }
                                                        }
                                                    }
                                                ]
                                            }
                                        },
                                        Right = new SumExpression() {
                                            Sum = new Sum() {
                                                Entries = [
                                                    new SumEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = "a"
                                                            }
                                                        }
                                                    },
                                                    new SumEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = "b"
                                                            }
                                                        }
                                                    },
                                                    new SumEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = "d"
                                                            }
                                                        }
                                                    }
                                                ]
                                            }
                                        }
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["a"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        },
                        ["b"] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        }
                    };
            }
            
            public class MultipleKeysAndSchemaValue : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new ExpressionKey() {
                                    Expression = new SumExpression() {
                                        Sum = new Sum() {
                                            Entries = [
                                                new SumEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "a"
                                                        }
                                                    }
                                                },
                                                new SumEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "b"
                                                        }
                                                    }
                                                }
                                            ]
                                        }
                                    }
                                },
                                Value = new SchemaExpression() {
                                    Schema = new Schema() {
                                        Entries = [
                                            new SchemaEntry() {
                                                Key = new StaticKey() {
                                                    Name = new Label() {
                                                        Value = "c"
                                                    }
                                                }
                                            },
                                            new SchemaEntry() {
                                                Key = new StaticKey() {
                                                    Name = new Label() {
                                                        Value = "d"
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["a"] = new MappingNode.Value() {
                            Node = new MappingNode() {
                                ["c"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                },
                                ["d"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                },
                            }
                        },
                        ["b"] = new MappingNode.Value() {
                            Node = new MappingNode() {
                                ["c"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                },
                                ["d"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                },
                            }
                        },
                    };
            }
            
            public class SchemaUnion : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = "key"
                                    }
                                },
                                Value = new UnionExpression() {
                                    Left = new SchemaExpression() {
                                        Schema = new Schema() {
                                            Entries = [
                                                new SchemaEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "a"
                                                        }
                                                    }
                                                }
                                            ]
                                        }
                                    },
                                    Right = new SchemaExpression() {
                                        Schema = new Schema() {
                                            Entries = [
                                                new SchemaEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "b"
                                                        }
                                                    }
                                                }
                                            ]
                                        }
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["key"] = new MappingNode.Value() {
                            Node = new MappingNode() {
                                ["a"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                },
                                ["b"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                }
                            }
                        },
                    };
            }
            
            public class SchemaIntersection : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = "key"
                                    }
                                },
                                Value = new IntersectionExpression() {
                                    Left = new SchemaExpression() {
                                        Schema = new Schema() {
                                            Entries = [
                                                new SchemaEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "a"
                                                        }
                                                    }
                                                },
                                                new SchemaEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "b"
                                                        }
                                                    }
                                                },
                                                new SchemaEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "c"
                                                        }
                                                    }
                                                }
                                            ]
                                        }
                                    },
                                    Right = new SchemaExpression() {
                                        Schema = new Schema() {
                                            Entries = [
                                                new SchemaEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "a"
                                                        }
                                                    }
                                                },
                                                new SchemaEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "b"
                                                        }
                                                    }
                                                },
                                                new SchemaEntry() {
                                                    Key = new StaticKey() {
                                                        Name = new Label() {
                                                            Value = "d"
                                                        }
                                                    }
                                                }
                                            ]
                                        }
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["key"] = new MappingNode.Value() {
                            Node = new MappingNode() {
                                ["a"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                },
                                ["b"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                }
                            }
                        },
                    };
            }
            
            public class SumValue : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = "key"
                                    }
                                },
                                Value = new SumExpression() {
                                    Sum = new Sum() {
                                        Entries = [
                                            new SumEntry() {
                                                Key = new StaticKey() {
                                                    Name = new Label() {
                                                        Value = "a"
                                                    }
                                                }
                                            },
                                            new SumEntry() {
                                                Key = new StaticKey() {
                                                    Name = new Label() {
                                                        Value = "b"
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                }
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["key"] = new MappingNode.Value() {
                            Node = new SumNode() {
                                ["a"] = new SumNode.Value(),
                                ["b"] = new SumNode.Value()
                            }
                        },
                    };
            }
            
            public class SingleScalarWithSingleDocumentation : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = "key"
                                    }
                                },
                                DocumentationLines = [
                                    new DocumentationLine() {
                                        Value = "hi"
                                    }
                                ]
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["key"] = new MappingNode.Value() {
                            Node = new ScalarNode(),
                            Documentation = ["hi"]
                        }
                    };
            }
            
            public class SingleScalarWithSingleDocumentationAndLeftSpace : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = "key"
                                    }
                                },
                                DocumentationLines = [
                                    new DocumentationLine() {
                                        Value = " hi"
                                    }
                                ]
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["key"] = new MappingNode.Value() {
                            Node = new ScalarNode(),
                            Documentation = ["hi"]
                        }
                    };
            }
            
            public class SingleScalarWithMultipleDocumentation : TestSet {
                protected override Schema Schema
                    => new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = "key"
                                    }
                                },
                                DocumentationLines = [
                                    new DocumentationLine() {
                                        Value = "hi"
                                    },
                                    new DocumentationLine() {
                                        Value = "bye"
                                    }
                                ]
                            }
                        ]
                    };

                protected override INode SchemaNode
                    => new MappingNode() {
                        ["key"] = new MappingNode.Value() {
                            Node = new ScalarNode(),
                            Documentation = ["hi", "bye"]
                        }
                    };
            }
        }

        [Fact]
        public void Analyze_GlobalVariable_Works() {
            const string referenceKey = "reference";
            const string innerKey = "a";
            
            var referencedSchema = new ExpressionDeclaration() {
                Name = new Label() {
                    Value = "referencedSchema"
                },
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = innerKey
                                    }
                                }
                            }
                        ]
                    }
                }
            };

            var referencedSchemaIdentifierReference = new Identifier() {
                Value = nameof(referencedSchema)
            };
            
            var referencingSchema = new ExpressionDeclaration() {
                IsExport = true,
                Name = new Label() {
                    Value = "referencingSchema"
                },
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = referenceKey
                                    }
                                },
                                Value = new IdentifierExpression() {
                                    Identifier = referencedSchemaIdentifierReference
                                }
                            }
                        ]
                    }
                }
            };

            var file = new ZuhFile() {
                RootStatements = [
                    referencedSchema,
                    referencingSchema
                ]
            };

            var fileScope = new Scope();

            var scopeTracker = new ScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope
                },
                NodeToEnclosingScope = {
                    [referencedSchema] = fileScope,
                    [referencingSchema] = fileScope
                }
            };

            var symbolTracker = new SymbolTracker() {
                Symbols = {
                    [referencedSchemaIdentifierReference] = new ExpressionSymbol() {
                        Name = nameof(referencedSchema),
                        Expression = referencedSchema.Expression
                    }
                }
            };

            var analyzer = new UnitAnalyzer() {
                CompilationAnalyzer = null!,
                File = file,
                UnitId = "main.zuh",
                ScopeTracker = scopeTracker,
                SymbolTracker = symbolTracker,
            };

            var generator = new UnitGenerator() {
                Analyzer = analyzer
            };
                    
            var root = generator.Generate();
                    
            Assert.Equivalent(new MappingNode() {
                [nameof(referencingSchema)] = new MappingNode.Value() {
                    Node = new MappingNode() {
                        [referenceKey] = new MappingNode.Value() {
                            Node = new MappingNode() {
                                [innerKey] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                }
                            }
                        }
                    }
                }
            }, root);
        }
        
        [Fact]
        public void Analyze_FunctionCall_Works() {
            const string referenceKey = "reference";
            const string innerKey = "a";
            const string argumentKey = "b";

            var schemaParam = new FunctionParameter() {
                Name = new Label() {
                    Value = "schemaParam"
                },
                Type = FunctionParameter.FunctionParameterType.Schema
            };

            var schemaParamIdentifierReference = new Identifier() {
                Value = nameof(schemaParam)
            };
            
            var func = new FunctionDeclaration() {
                Name = new Label() {
                    Value = "referencedSchema"
                },
                Function = new Function() {
                    Parameters = [
                        schemaParam
                    ],
                    Expression = new SchemaExpression() {
                        Schema = new Schema() {
                            Entries = [
                                new SchemaEntry() {
                                    Key = new StaticKey() {
                                        Name = new Label() {
                                            Value = innerKey
                                        }
                                    },
                                    Value = new IdentifierExpression() {
                                        Identifier = schemaParamIdentifierReference
                                    }
                                }
                            ]
                        }
                    } 
                }
            };

            var functionIdentifierReference = new Identifier() {
                Value = nameof(func)
            };
            
            var referencingSchema = new ExpressionDeclaration() {
                IsExport = true,
                Name = new Label() {
                    Value = "referencingSchema"
                },
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
                                    Name = new Label() {
                                        Value = referenceKey
                                    }
                                },
                                Value = new FunctionInvocationExpression() {
                                    FunctionIdentifier = functionIdentifierReference,
                                    Arguments = [
                                        new SchemaExpression() {
                                            Schema = new Schema() {
                                                Entries = [
                                                    new SchemaEntry() {
                                                        Key = new StaticKey() {
                                                            Name = new Label() {
                                                                Value = argumentKey
                                                            }
                                                        }
                                                    }
                                                ]
                                            }
                                        }
                                    ]
                                }
                            }
                        ]
                    }
                }
            };

            var file = new ZuhFile() {
                RootStatements = [
                    func,
                    referencingSchema
                ]
            };

            var schemaParamSymbol = new FunctionParameterSymbol() {
                Name = nameof(schemaParam),
                FunctionParameter = schemaParam
            };
            
            var functionSymbol = new FunctionSymbol() {
                Name = nameof(func),
                Function = func.Function,
                Parameters = [
                    schemaParamSymbol
                ]
            };
            
            var fileScope = new Scope();

            var funcScope = new Scope();

            var scopeTracker = new ScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope,
                    [func.Function] = funcScope
                },
                NodeToEnclosingScope = {
                    [func] = fileScope,
                    [referencingSchema] = fileScope,
                    [functionIdentifierReference] = fileScope,
                    [schemaParamIdentifierReference] = funcScope,
                }
            };

            var symbolTracker = new SymbolTracker() {
                Symbols = {
                    [functionIdentifierReference] = functionSymbol,
                    [schemaParamIdentifierReference] = schemaParamSymbol
                }
            };

            var analyzer = new UnitAnalyzer() {
                CompilationAnalyzer = null!,
                File = file,
                UnitId = "main.zuh",
                ScopeTracker = scopeTracker,
                SymbolTracker = symbolTracker,
            };

            var generator = new UnitGenerator() {
                Analyzer = analyzer
            };
                    
            var root = generator.Generate();
                    
            Assert.Equivalent(new MappingNode() {
                [nameof(referencingSchema)] = new MappingNode.Value() {
                    Node = new MappingNode() {
                        [referenceKey] = new MappingNode.Value() {
                            Node = new MappingNode() {
                                [innerKey] = new MappingNode.Value() {
                                    Node = new MappingNode() {
                                        [argumentKey] = new MappingNode.Value() {
                                            Node = new ScalarNode()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }, root);
        }
    }
}