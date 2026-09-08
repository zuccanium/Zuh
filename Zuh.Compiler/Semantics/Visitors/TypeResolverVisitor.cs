using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Unit;
using Zuh.Compiler.Semantics.Types;

namespace Zuh.Compiler.Semantics.Visitors {
    /// <summary>
    /// populates a <see cref="UnitTypeTracker"/> with expression types.
    /// if the type of an expression cant be resolved, there will be no entry for that expression.
    /// </summary>
    /// <remarks>
    /// DO NOT CALL THIS ON A ROOT <see cref="ZuhFile"/>!!!
    /// there is no dependency resolution code; it expects every symbol it needs to have a resolved type already.
    /// </remarks>
    public class TypeResolverVisitor : Visitor {
        public required UnitTypeTracker UnitTypeTracker { get; init; }
        public required UnitSymbolTracker UnitSymbolTracker { get; init; }
        
        public required DiagnosticCollector Diagnostics { get; init; }
        
        private readonly Stack<(ZuhType Type, Expression Expression)> stack = [];
        
        protected override List<Overload> Overloads
            => [
                new Overload<BinaryExpression>((node, next) => {
                    next();

                    pop(out var right);
                    pop(out var left);

                    if((left, right) is (SchemaType, SchemaType))
                        declareType(node, new SchemaType());

                    else if((left, right) is (SumType, SumType))
                        declareType(node, new SumType());

                    else if((left, right) is ({ }, { }))
                        Diagnostics.Add(new InvalidBinaryOperatorError() {
                            LeftType = left,
                            RightType = right,
                            Operator = node switch {
                                UnionExpression => "|",
                                IntersectionExpression => "&",
                                _ => throw new NotImplementedException()
                            },
                            Location = node.SourceSpan
                        });
                }),
                new Overload<FunctionInvocationExpression>((node, next) => {
                    next();
                    
                    var symbol = UnitSymbolTracker.IdentifierToSymbol[node.FunctionIdentifier];

                    if(symbol is not FunctionDeclarationSymbol functionSymbol)
                        throw new Exception();

                    var functionArgumentTuples = stack.Reverse().ToList();
                    
                    stack.Clear();
                    
                    if(functionArgumentTuples.Count != functionSymbol.Parameters.Length) {
                        Diagnostics.Add(new InvalidFunctionArgumentCountError() {
                            ParameterCount = functionSymbol.Parameters.Length,
                            ArgumentCount = functionArgumentTuples.Count,
                            FunctionName = functionSymbol.Name,
                            Location = node.SourceSpan
                        });

                        return;
                    }

                    foreach(var (parameter, argumentTuple) in functionSymbol.Parameters.Zip(functionArgumentTuples)) {
                        if(argumentTuple.Type != parameter.Type)
                            Diagnostics.Add(new InvalidFunctionArgumentError() {
                                ExpectedType = parameter.Type!,
                                ProvidedType = argumentTuple.Type,
                                ParameterName = parameter.Name,
                                Location = argumentTuple.Expression.SourceSpan
                            });
                    }

                    if(functionSymbol.Type is not FunctionType functionType)
                        throw new Exception("function symbol doesnt have function type");
                    
                    declareType(node, functionType.ReturnType);
                }),
                new Overload<ArrayExpression>((node, next) => {
                    next();
                    
                    pop(out var inner);
                    
                    declareType(node, new ArrayType() {
                        Inner = inner! with { }
                    });
                }),
                new Overload<ParenthesizedExpression>((node, next) => {
                    next();
                    
                    pop(out var inner);
                    
                    if(inner is { })
                        declareType(node, inner);
                }),
                new Overload<SchemaExpression>((node, next) => {
                    declareType(node, new SchemaType());
                }),
                new Overload<SumExpression>((node, next) => {
                    declareType(node, new SumType());
                }),
                new Overload<IdentifierExpression>((node, next) => {
                    var symbol = UnitSymbolTracker.IdentifierToSymbol[node.Identifier];

                    if(symbol.Type is not { } type) {
                        Diagnostics.Add(new IdentifierTypeResolutionError() {
                            Identifier = node.Identifier.Value,
                            Location = node.Identifier.SourceSpan
                        });

                        return;
                    }
                    
                    declareType(node, type);
                }),
                new Overload<FunctionParameter>((node, next) => {
                    var symbol = UnitSymbolTracker.NodeToPersonalSymbol[node];

                    symbol.Type = node.Type switch {
                        FunctionParameter.FunctionParameterType.Schema => new SchemaType(),
                        FunctionParameter.FunctionParameterType.Sum => new SumType(),
                        _ => throw new NotImplementedException()
                    };
                }),
                new Overload<FunctionDeclaration>((node, next) => {
                    next();
                    
                    pop(out var returnTypeMaybe);

                    if(returnTypeMaybe is not { } returnType)
                        throw new Exception();
                    
                    var symbol = UnitSymbolTracker.NodeToPersonalSymbol[node];

                    if(symbol is not FunctionDeclarationSymbol functionSymbol)
                        throw new Exception();
                    
                    symbol.Type = new FunctionType() {
                        ReturnType = returnType,
                        ParameterTypes = [
                            ..functionSymbol.Parameters
                                .Select(parameter => parameter.Type!)
                        ]
                    };
                }),
                new Overload<ExpressionDeclaration>((node, next) => {
                    next();
                    
                    pop(out var typeMaybe);

                    if(typeMaybe is not { } type)
                        throw new Exception();
                    
                    var symbol = UnitSymbolTracker.NodeToPersonalSymbol[node];

                    symbol.Type = type;
                })
            ];

        private void pop(out ZuhType? type, out Expression? expression) {
            if(stack.TryPop(out var poppedTuple)) {
                (type, expression) = poppedTuple;

                return;
            }
            
            type = null;
            expression = null;
        }

        private void pop(out ZuhType? type)
            => pop(out type, out _);
        
        private void declareType(Expression node, ZuhType type) {
            UnitTypeTracker.ExpressionToType[node] = type;

            stack.Push((type, node));
        }
    }
}