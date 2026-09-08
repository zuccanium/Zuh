using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Unit;
using Zuh.Compiler.Semantics.Types;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Tests.Semantics.Visitors {
    public class TypeResolverVisitorTests {
        public class Visit_ValidBinaryExpression_Works_Data : TheoryData<BinaryExpression, ZuhType> {
            public Visit_ValidBinaryExpression_Works_Data() {
                var schemaExpression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = []
                    }
                };
                
                var sumExpression = new SumExpression() {
                    Sum = new Sum() {
                        Entries = []
                    }
                };

                var unionExpression = new UnionExpression() {
                    Left = null!,
                    Right = null!
                };
                
                var intersectionExpression = new IntersectionExpression() {
                    Left = null!,
                    Right = null!
                };
                
                add(new SchemaType(), unionExpression, schemaExpression);
                add(new SumType(), unionExpression, sumExpression);
                add(new SchemaType(), intersectionExpression, schemaExpression);
                add(new SumType(), intersectionExpression, sumExpression);
            }

            private void add(ZuhType type, BinaryExpression expression, Expression side)
                => Add(
                    expression with {
                        Left = side with { },
                        Right = side with { }
                    },
                    type
                );
        }
        
        [Theory]
        [ClassData(typeof(Visit_ValidBinaryExpression_Works_Data))]
        public void Visit_ValidBinaryExpression_Works(BinaryExpression binaryExpression, ZuhType type) {
            var unitTypeTracker = new UnitTypeTracker();

            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = unitTypeTracker,
                UnitSymbolTracker = null!,
                Diagnostics = null!
            };
            
            visitor.Visit(binaryExpression);

            var expectedDictionary = new Dictionary<Expression, ZuhType>() {
                [binaryExpression] = type,
                [binaryExpression.Left] = type,
                [binaryExpression.Right] = type
            };
            
            Assert.Equivalent(expectedDictionary, unitTypeTracker.ExpressionToType);
        }
        
        public class Visit_InvalidBinaryExpression_CreatesDiagnostic_Data : TheoryData<Expression, Diagnostic> {
            public Visit_InvalidBinaryExpression_CreatesDiagnostic_Data() {
                var schemaExpression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = []
                    }
                };
                
                var sumExpression = new SumExpression() {
                    Sum = new Sum() {
                        Entries = []
                    }
                };
                
                add(
                    new IntersectionExpression() {
                        Left = sumExpression,
                        Right = schemaExpression
                    },
                    "&",
                    new SumType(),
                    new SchemaType()
                );
                
                add(
                    new IntersectionExpression() {
                        Left = schemaExpression,
                        Right = sumExpression
                    },
                    "&",
                    new SchemaType(),
                    new SumType()
                );
                
                add(
                    new UnionExpression() {
                        Left = sumExpression,
                        Right = schemaExpression
                    },
                    "|",
                    new SumType(),
                    new SchemaType()
                );
                
                add(
                    new UnionExpression() {
                        Left = schemaExpression,
                        Right = sumExpression
                    },
                    "|",
                    new SchemaType(),
                    new SumType()
                );
            }

            private void add(BinaryExpression expression, string operatorSymbol, ZuhType leftType, ZuhType rightType)
                => Add(
                    expression,
                    new InvalidBinaryOperatorError() {
                        LeftType = leftType,
                        RightType = rightType,
                        Operator = operatorSymbol,
                        Location = expression.SourceSpan
                    }
                );
        }
        
        [Theory]
        [ClassData(typeof(Visit_InvalidBinaryExpression_CreatesDiagnostic_Data))]
        public void Visit_InvalidBinaryExpression_CreatesDiagnostic(Expression binaryExpression, Diagnostic expectedDiagnostic) {
            var diagnosticCollector = new DiagnosticCollector();

            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = new UnitTypeTracker(),
                UnitSymbolTracker = null!,
                Diagnostics = diagnosticCollector
            };
            
            visitor.Visit(binaryExpression);

            Assert.Equivalent((List<Diagnostic>)[expectedDiagnostic], diagnosticCollector);
        }

        public class Visit_ValidFunctionInvocationExpression_Works_Data : TheoryData<FunctionInvocationExpression, ZuhType> {
            public Visit_ValidFunctionInvocationExpression_Works_Data() {
                add(new SchemaType());
                add(new SumType());
            }

            private void add(ZuhType type)
                => Add(
                    new FunctionInvocationExpression() {
                        FunctionIdentifier = new Identifier() {
                            Value = "func"
                        },
                        Arguments = []
                    },
                    type
                );
        }

        [Theory]
        [ClassData(typeof(Visit_ValidFunctionInvocationExpression_Works_Data))]
        public void Visit_ValidFunctionInvocationExpression_Works(FunctionInvocationExpression functionInvocationExpression, ZuhType type) {
            var unitTypeTracker = new UnitTypeTracker();

            var functionSymbol = new FunctionDeclarationSymbol() {
                Name = "func",
                Type = new FunctionType() {
                    ReturnType = type,
                    ParameterTypes = []
                },
                Parameters = [],
                FunctionDeclaration = null!,
                UnitId = ""
            };

            var unitSymbolTracker = new UnitSymbolTracker() {
                IdentifierToSymbol = {
                    [functionInvocationExpression.FunctionIdentifier] = functionSymbol
                }
            };

            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = unitTypeTracker,
                UnitSymbolTracker = unitSymbolTracker,
                Diagnostics = null!
            };
            
            visitor.Visit(functionInvocationExpression);

            var expectedDictionary = new Dictionary<Expression, ZuhType>() {
                [functionInvocationExpression] = type
            };
            
            Assert.Equivalent(expectedDictionary, unitTypeTracker.ExpressionToType);
        }

        public class Visit_InvalidFunctionInvocationExpressionBadArgumentCount_CreatesDiagnostic_Data
            : TheoryData<
                FunctionInvocationExpression,
                FunctionDeclarationSymbol,
                Diagnostic
            >
        {
            public Visit_InvalidFunctionInvocationExpressionBadArgumentCount_CreatesDiagnostic_Data() {
                for(var i = 0; i < 5; i++)
                    for(var j = 0; j < 5; j++)
                        if(i != j)
                            add(i, j);
            }

            private void add(int arguments, int parameters) {
                var invocationSourceSpan = new SourceSpan() {
                    Start = 10,
                    End = 11
                };
                
                Add(
                    new FunctionInvocationExpression() {
                        Arguments = [
                            ..Enumerable
                                .Range(0, arguments)
                                .Select(_ => new SchemaExpression() {
                                    Schema = new Schema() {
                                        Entries = []
                                    }
                                })
                        ],
                        FunctionIdentifier = new Identifier() {
                            Value = "func"
                        },
                        SourceSpan = invocationSourceSpan
                    },
                    new FunctionDeclarationSymbol() {
                        Name = "func",
                        UnitId = "",
                        Type = new FunctionType() {
                            ReturnType = new SchemaType(),
                            ParameterTypes = [
                                ..Enumerable
                                    .Range(0, parameters)
                                    .Select(_ => new SchemaType())
                            ]
                        },
                        Parameters = [
                            ..Enumerable
                                .Range(0, parameters)
                                .Select(n => new FunctionParameterSymbol() {
                                    Name = $"param{n}",
                                    UnitId = "",
                                    FunctionParameter = null!,
                                    Type = new SchemaType()
                                })
                        ],
                        FunctionDeclaration = null!
                    },
                    new InvalidFunctionArgumentCountError() {
                        FunctionName = "func",
                        ArgumentCount = arguments,
                        ParameterCount = parameters,
                        Location = invocationSourceSpan
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Visit_InvalidFunctionInvocationExpressionBadArgumentCount_CreatesDiagnostic_Data))]
        public void Visit_InvalidFunctionInvocationExpressionBadArgumentCount_CreatesDiagnostic(
            FunctionInvocationExpression functionInvocationExpression,
            FunctionDeclarationSymbol functionDeclarationSymbol,
            Diagnostic expectedDiagnostic
        ) {
            var diagnosticCollector = new DiagnosticCollector();

            var unitSymbolTracker = new UnitSymbolTracker() {
                IdentifierToSymbol = {
                    [functionInvocationExpression.FunctionIdentifier] = functionDeclarationSymbol
                }
            };
            
            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = new UnitTypeTracker(),
                UnitSymbolTracker = unitSymbolTracker,
                Diagnostics = diagnosticCollector
            };
            
            visitor.Visit(functionInvocationExpression);

            Assert.Equivalent((List<Diagnostic>)[expectedDiagnostic], diagnosticCollector);
        }

        public class Visit_InvalidFunctionInvocationExpressionBadArgumentTypes_CreatesDiagnostic_Data
            : TheoryData<
                FunctionInvocationExpression,
                FunctionDeclarationSymbol,
                List<Diagnostic>
            >
        {
            public Visit_InvalidFunctionInvocationExpressionBadArgumentTypes_CreatesDiagnostic_Data() {
                var schemaExpression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = []
                    }
                };
                
                var sumExpression = new SumExpression() {
                    Sum = new Sum() {
                        Entries = []
                    }
                };

                var max = 2;

                void recur(List<ZuhType> parameterTypes, List<(Expression Expression, ZuhType Type)> arguments) {
                    if(parameterTypes.Count == max) {
                        add(parameterTypes, arguments);

                        return;
                    }

                    var paramsWithSum = (List<ZuhType>)[
                        ..parameterTypes,
                        new SumType()
                    ];

                    var paramsWithSchema = (List<ZuhType>) [
                        ..parameterTypes,
                        new SchemaType()
                    ];

                    var argumentsWithSchema = (List<(Expression Expression, ZuhType Type)>)[
                        ..arguments,
                        (Expression: schemaExpression with { }, Type: new SchemaType())
                    ];
                    
                    var argumentsWithSum = (List<(Expression Expression, ZuhType Type)>)[
                        ..arguments,
                        (Expression: sumExpression with { }, Type: new SumType())
                    ];
                    
                    // call me a fluffy fox the way i keep having these tail calls
                    recur(paramsWithSchema, argumentsWithSchema);
                    recur(paramsWithSum, argumentsWithSchema);
                    recur(paramsWithSchema, argumentsWithSum);
                    recur(paramsWithSum, argumentsWithSum);
                }
                
                recur([], []);
            }

            private void add(List<ZuhType> parameterTypes, List<(Expression Expression, ZuhType Type)> arguments) {
                var invocation = new FunctionInvocationExpression() {
                    Arguments = [
                        ..arguments
                            .Select(argument => argument.Expression)
                    ],
                    FunctionIdentifier = new Identifier() {
                        Value = "func"
                    }
                };

                var declarationSymbol = new FunctionDeclarationSymbol() {
                    Name = "func",
                    UnitId = "",
                    Type = new FunctionType() {
                        ReturnType = new SchemaType(),
                        ParameterTypes = parameterTypes
                    },
                    Parameters = [
                        ..parameterTypes
                            .Select((parameterType, n) => new FunctionParameterSymbol() {
                                Name = $"param{n}",
                                UnitId = "",
                                FunctionParameter = null!,
                                Type = parameterType
                            })
                    ],
                    FunctionDeclaration = null!
                };

                var diagnostics = new List<Diagnostic>();

                foreach(var (parameterSymbol, argumentTuple) in declarationSymbol.Parameters.Zip(arguments))
                    if(parameterSymbol.Type != argumentTuple.Type)
                        diagnostics.Add(new InvalidFunctionArgumentError() {
                            ExpectedType = parameterSymbol.Type!,
                            ProvidedType = argumentTuple.Type,
                            Location = argumentTuple.Expression.SourceSpan,
                            ParameterName = parameterSymbol.Name
                        });
                
                Add(invocation, declarationSymbol, diagnostics);
            }
        }

        [Theory]
        [ClassData(typeof(Visit_InvalidFunctionInvocationExpressionBadArgumentTypes_CreatesDiagnostic_Data))]
        public void Visit_InvalidFunctionInvocationExpressionBadArgumentTypes_CreatesDiagnostic(
            FunctionInvocationExpression functionInvocationExpression,
            FunctionDeclarationSymbol functionDeclarationSymbol,
            List<Diagnostic> expectedDiagnostics
        ) {
            var diagnosticCollector = new DiagnosticCollector();

            var unitSymbolTracker = new UnitSymbolTracker() {
                IdentifierToSymbol = {
                    [functionInvocationExpression.FunctionIdentifier] = functionDeclarationSymbol
                }
            };
            
            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = new UnitTypeTracker(),
                UnitSymbolTracker = unitSymbolTracker,
                Diagnostics = diagnosticCollector
            };
            
            visitor.Visit(functionInvocationExpression);

            Assert.Equivalent(expectedDiagnostics, diagnosticCollector);
        }

        public class Visit_ValidParenthesizedExpression_Works_Data : TheoryData<ParenthesizedExpression, ZuhType> {
            public Visit_ValidParenthesizedExpression_Works_Data() {
                Add(
                    new ParenthesizedExpression() {
                        Expression = new SchemaExpression() {
                            Schema = new Schema() {
                                Entries = []
                            }
                        }
                    },
                    new SchemaType()
                );
                
                Add(
                    new ParenthesizedExpression() {
                        Expression = new SumExpression() {
                            Sum = new Sum() {
                                Entries = []
                            }
                        }
                    },
                    new SumType()
                );
            }
        }

        [Theory]
        [ClassData(typeof(Visit_ValidParenthesizedExpression_Works_Data))]
        public void Visit_ValidParenthesizedExpression_Works(ParenthesizedExpression expression, ZuhType type) {
            var unitTypeTracker = new UnitTypeTracker();

            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = unitTypeTracker,
                UnitSymbolTracker = null!,
                Diagnostics = null!
            };
            
            visitor.Visit(expression);

            var expectedDictionary = new Dictionary<Expression, ZuhType>() {
                [expression] = type,
                [expression.Expression] = type
            };
            
            Assert.Equivalent(expectedDictionary, unitTypeTracker.ExpressionToType);
        }

        private void visit_ValidDefinitionExpression_Works(Expression expression, ZuhType type) {
            var unitTypeTracker = new UnitTypeTracker();

            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = unitTypeTracker,
                UnitSymbolTracker = null!,
                Diagnostics = null!
            };
            
            visitor.Visit(expression);

            var expectedDictionary = new Dictionary<Expression, ZuhType>() {
                [expression] = type
            };
            
            Assert.Equal(expectedDictionary, unitTypeTracker.ExpressionToType);
        }

        [Fact]
        public void Visit_ValidSchemaExpression_Works()
            => visit_ValidDefinitionExpression_Works(
                new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = []
                    }
                },
                new SchemaType()
            );
        
        [Fact]
        public void Visit_ValidSumExpression_Works()
            => visit_ValidDefinitionExpression_Works(
                new SumExpression() {
                    Sum = new Sum() {
                        Entries = []
                    }
                },
                new SumType()
            );
        
        public class Visit_ValidArrayExpression_Works_Data : TheoryData<Expression, Dictionary<Expression, ZuhType>> {
            public Visit_ValidArrayExpression_Works_Data() {
                for(var i = 0; i < 3; i++) {
                    add(
                        i,
                        new SchemaExpression() {
                            Schema = null!
                        },
                        new SchemaType()
                    );
                    
                    add(
                        i,
                        new SumExpression() {
                            Sum = null!
                        },
                        new SumType()
                    );
                }
            }

            private void add(int arrayLayers, Expression expression, ZuhType type) {
                var wrappedExpression = expression;
                var wrappedType = type;

                var map = new Dictionary<Expression, ZuhType>() {
                    [expression] = type
                };

                for(var i = 0; i < arrayLayers; i++) {
                    wrappedExpression = new ArrayExpression() {
                        Expression = wrappedExpression
                    };
                    
                    wrappedType = new ArrayType() {
                        Inner = wrappedType
                    };

                    map[wrappedExpression] = wrappedType;
                }
                
                Add(wrappedExpression, map);
            }
        }

        [Theory]
        [ClassData(typeof(Visit_ValidArrayExpression_Works_Data))]
        public void Visit_ValidArrayExpression_Works(Expression expression, Dictionary<Expression, ZuhType> expectedDictionary) {
            var unitTypeTracker = new UnitTypeTracker();

            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = unitTypeTracker,
                UnitSymbolTracker = null!,
                Diagnostics = null!
            };
            
            visitor.Visit(expression);

            Assert.Equivalent(expectedDictionary, unitTypeTracker.ExpressionToType);
        }

        public class Visit_ValidFunctionParameter_Works_Data : TheoryData<FunctionParameter, ZuhType> {
            public Visit_ValidFunctionParameter_Works_Data() {
                Add(
                    new FunctionParameter() {
                        Name = null!,
                        Type = FunctionParameter.FunctionParameterType.Schema
                    },
                    new SchemaType()
                );
                
                Add(
                    new FunctionParameter() {
                        Name = null!,
                        Type = FunctionParameter.FunctionParameterType.Sum
                    },
                    new SumType()
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Visit_ValidFunctionParameter_Works_Data))]
        public void Visit_ValidFunctionParameter_Works(FunctionParameter functionParameter, ZuhType type) {
            var symbol = new FunctionParameterSymbol() {
                Name = "idk",
                UnitId = "",
                FunctionParameter = functionParameter,
                Type = null
            };

            var unitSymbolTracker = new UnitSymbolTracker() {
                NodeToPersonalSymbol = {
                    [functionParameter] = symbol
                }
            };
            
            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = null!,
                UnitSymbolTracker = unitSymbolTracker,
                Diagnostics = null!
            };
            
            visitor.Visit(functionParameter);
            
            Assert.Equivalent(type, symbol.Type);
        }

        public class Visit_ValidIdentifierExpression_Works_Data : TheoryData<Symbol, ZuhType> {
            public Visit_ValidIdentifierExpression_Works_Data() {
                addExpressionDeclaration(new SchemaType());
                addExpressionDeclaration(new SumType());
            }

            private void addExpressionDeclaration(ZuhType type)
                => Add(
                    new ExpressionDeclarationSymbol() {
                        Name = "",
                        UnitId = "",
                        Type = type,
                        ExpressionDeclaration = null!
                    },
                    type
                );
        }

        [Theory]
        [ClassData(typeof(Visit_ValidIdentifierExpression_Works_Data))]
        public void Visit_ValidIdentifierExpression_Works(Symbol symbol, ZuhType type) {
            var identifier = new Identifier() {
                Value = "identifier"
            };
            
            var identifierExpression = new IdentifierExpression() {
                Identifier = identifier
            };

            var unitSymbolTracker = new UnitSymbolTracker() {
                IdentifierToSymbol = {
                    [identifier] = symbol
                }
            };

            var unitTypeTracker = new UnitTypeTracker();
            
            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = unitTypeTracker,
                UnitSymbolTracker = unitSymbolTracker,
                Diagnostics = null!
            };

            visitor.Visit(identifierExpression);

            var expectedDictionary = new Dictionary<Expression, ZuhType>() {
                [identifierExpression] = type
            };
            
            Assert.Equivalent(expectedDictionary, unitTypeTracker.ExpressionToType);
        }

        [Fact]
        public void Visit_InvalidIdentifierExpression_CreatesDiagnostic() {
            var identifier = new Identifier() {
                Value = "identifier",
                SourceSpan = new SourceSpan() {
                    Start = 10,
                    End = 122
                }
            };
            
            var identifierExpression = new IdentifierExpression() {
                Identifier = identifier
            };

            var symbol = new ExpressionDeclarationSymbol() {
                Name = "",
                UnitId = "",
                Type = null,
                ExpressionDeclaration = null!
            };
            
            var unitSymbolTracker = new UnitSymbolTracker() {
                IdentifierToSymbol = {
                    [identifier] = symbol
                }
            };

            var diagnosticsCollector = new DiagnosticCollector();
            
            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = null!,
                UnitSymbolTracker = unitSymbolTracker,
                Diagnostics = diagnosticsCollector
            };
            
            visitor.Visit(identifierExpression);

            var expectedDiagnostic = new IdentifierTypeResolutionError() {
                Identifier = identifier.Value,
                Location = identifier.SourceSpan
            };
            
            Assert.Equivalent((List<Diagnostic>)[expectedDiagnostic], diagnosticsCollector);
        }

        public class Visit_ValidExpressionDeclaration_Works_Data : TheoryData<Expression, ZuhType> {
            public Visit_ValidExpressionDeclaration_Works_Data() {
                Add(
                    new SchemaExpression() {
                        Schema = new Schema() {
                            Entries = []
                        }
                    },
                    new SchemaType()
                );
                
                Add(
                    new SumExpression() {
                        Sum = new Sum() {
                            Entries = []
                        }
                    },
                    new SumType()
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Visit_ValidExpressionDeclaration_Works_Data))]
        public void Visit_ValidExpressionDeclaration_Works(Expression expression, ZuhType type) {
            var expressionDeclaration = new ExpressionDeclaration() {
                Name = new Label() {
                    Value = "expressionDeclaration"
                },
                Expression = expression
            };

            var expressionDeclarationSymbol = new ExpressionDeclarationSymbol() {
                Name = expressionDeclaration.Name.Value,
                UnitId = "",
                ExpressionDeclaration = expressionDeclaration
            };

            var unitTypeTracker = new UnitTypeTracker();
            
            var unitSymbolTracker = new UnitSymbolTracker() {
                NodeToPersonalSymbol = {
                    [expressionDeclaration] = expressionDeclarationSymbol
                }
            };

            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = unitTypeTracker,
                UnitSymbolTracker = unitSymbolTracker,
                Diagnostics = null!
            };
            
            visitor.Visit(expressionDeclaration);

            var expectedDictionary = new Dictionary<Expression, ZuhType>() {
                [expression] = type
            };
            
            Assert.Equivalent(type, expressionDeclarationSymbol.Type);
            Assert.Equivalent(expectedDictionary, unitTypeTracker.ExpressionToType);
        }

        // this should absolutely be a theory
        // i just really dont want to write the monstrous generation code for the data
        // sorry
        // this will probably work
        [Fact]
        public void Visit_ValidFunctionDeclaration_Works() {
            var functionDeclaration = new FunctionDeclaration() {
                Name = new Label() {
                    Value = "functionDeclaration"
                },
                Function = new Function() {
                    Parameters = [
                        new FunctionParameter() {
                            Name = new Label() {
                                Value = "param0"
                            },
                            Type = FunctionParameter.FunctionParameterType.Schema
                        },
                        new FunctionParameter() {
                            Name = new Label() {
                                Value = "param1"
                            },
                            Type = FunctionParameter.FunctionParameterType.Sum
                        }
                    ],
                    Expression = new SchemaExpression() {
                        Schema = new Schema() {
                            Entries = []
                        }
                    }
                }
            };

            var functionDeclarationSymbol = new FunctionDeclarationSymbol() {
                Name = functionDeclaration.Name.Value,
                UnitId = "",
                Parameters = [
                    new FunctionParameterSymbol() {
                        Name = "param0",
                        UnitId = "",
                        FunctionParameter = null!
                    },
                    new FunctionParameterSymbol() {
                        Name = "param1",
                        UnitId = "",
                        FunctionParameter = null!
                    }
                ],
                FunctionDeclaration = functionDeclaration
            };

            var unitTypeTracker = new UnitTypeTracker();
            
            var unitSymbolTracker = new UnitSymbolTracker() {
                NodeToPersonalSymbol = {
                    [functionDeclaration] = functionDeclarationSymbol,
                    [functionDeclaration.Function.Parameters[0]] = functionDeclarationSymbol.Parameters[0],
                    [functionDeclaration.Function.Parameters[1]] = functionDeclarationSymbol.Parameters[1]
                }
            };

            var visitor = new TypeResolverVisitor() {
                UnitTypeTracker = unitTypeTracker,
                UnitSymbolTracker = unitSymbolTracker,
                Diagnostics = null!
            };
            
            visitor.Visit(functionDeclaration);

            var expectedDictionary = new Dictionary<Expression, ZuhType>() {
                [functionDeclaration.Function.Expression] = new SchemaType()
            };

            var functionType = new FunctionType() {
                ParameterTypes = [
                    new SchemaType(),
                    new SumType()
                ],
                ReturnType = new SchemaType()
            };
            
            Assert.Equivalent(functionType, functionDeclarationSymbol.Type);
            Assert.Equivalent(expectedDictionary, unitTypeTracker.ExpressionToType);
        }
    }
}