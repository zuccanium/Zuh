using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Pidgin.Expression;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, IdentifierExpression> IdentifierExpression = null!;
        internal static Parser<char, SchemaExpression> SchemaExpression = null!;
        internal static Parser<char, SumExpression> SumExpression = null!;
        internal static Parser<char, FunctionInvocationExpression> FunctionInvocationExpression = null!;
        internal static Parser<char, Func<Expression, Expression>> ArrayProtoExpression = null!;
        internal static Parser<char, Func<Expression, Expression, Expression>> IntersectionProtoExpression = null!;
        internal static Parser<char, Func<Expression, Expression, Expression>> UnionProtoExpression = null!;
        internal static Parser<char, Expression> Expression = null!;

        private static void initializeExpressions() {
            IdentifierExpression
                = Identifier.Select(identifier => new IdentifierExpression() {
                    Identifier = identifier
                });
            
            SchemaExpression
                = Schema.Select(schema => new SchemaExpression() {
                    Schema = schema
                });
            
            SumExpression
                = Sum.Select(sum => new SumExpression() {
                    Sum = sum
                });
            
            ArrayProtoExpression
                = Token("[]")
                    .Select<Func<Expression, Expression>>(
                        _ => expression => new ArrayExpression() {
                            Expression = expression
                        });

            FunctionInvocationExpression
                = Map(
                    (identifier, arguments) => new FunctionInvocationExpression() {
                        FunctionIdentifier = identifier,
                        Arguments = [..arguments]
                    },
                    Identifier,
                    Rec(() => Expression)
                        .Separated(EntrySeparator)
                        .Between(
                            Token("("),
                            Token(")")
                        )
                );
            
            IntersectionProtoExpression
                = Token("&")
                    .Select<Func<Expression, Expression, Expression>>(
                        _ => (left, right) => new IntersectionExpression() {
                            Left = left,
                            Right = right
                        });
            
            UnionProtoExpression
                = Token("|")
                    .Select<Func<Expression, Expression, Expression>>(
                        _ => (left, right) => new UnionExpression() {
                            Left = left,
                            Right = right
                        });
            
            Expression
                = WithLocation(
                    ExpressionParser.Build<char, Expression>(
                        _ => OneOf(
                            Try(FunctionInvocationExpression.Cast<Expression>()),
                            Try(IdentifierExpression.Cast<Expression>()),
                            Try(SchemaExpression.Cast<Expression>()),
                            Try(SumExpression.Cast<Expression>())
                        ),
                        [
                            Operator.Postfix(ArrayProtoExpression),
                            Operator.InfixL(IntersectionProtoExpression)
                        ]
                    )
                );
        }
    }
}