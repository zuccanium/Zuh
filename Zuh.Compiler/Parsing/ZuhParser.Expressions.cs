using Pidgin;
using static Pidgin.Parser;
using Pidgin.Expression;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, IdentifierExpression> IdentifierExpression = null!;
        internal static Parser<char, SchemaExpression> SchemaExpression = null!;
        internal static Parser<char, SumExpression> SumExpression = null!;
        
        internal static Parser<char, ParenthesizedExpression> ParenthesizedExpression = null!;
        internal static Parser<char, Func<Expression, Expression>> ArrayProtoExpression = null!;
        
        internal static Parser<char, FunctionInvocationExpression> FunctionInvocationExpression = null!;
        
        internal static Parser<char, Func<Expression, Expression, Expression>> IntersectionProtoExpression = null!;
        internal static Parser<char, Func<Expression, Expression, Expression>> UnionProtoExpression = null!;
        
        internal static Parser<char, Expression> Expression = null!;

        private static void initializeExpressions() {
            IdentifierExpression
                = (
                    from identifier in Identifier
                    select new IdentifierExpression() {
                        Identifier = identifier,
                        SourceSpan = identifier.SourceSpan
                    }
                );

            SchemaExpression
                = (
                    from schema in Schema
                    select new SchemaExpression() {
                        Schema = schema,
                        SourceSpan = schema.SourceSpan
                    }
                );
            
            SumExpression
                = (
                    from sum in Sum
                    select new SumExpression() {
                        Sum = sum,
                        SourceSpan = sum.SourceSpan
                    }
                );
            
            ParenthesizedExpression
                = (
                    from openParenthesis in Token("(")
                    from expression in Rec(() => Expression)
                    from closeParenthesis in Token(")")
                    select new ParenthesizedExpression() {
                        Expression = expression,
                        SourceSpan = openParenthesis.SourceSpan - closeParenthesis.SourceSpan
                    }
                );

            // this would be a proto expression if anonymous functions were supported
            // they are not
            FunctionInvocationExpression
                = (
                    from identifier in Identifier
                    from openParenthesis in Token("(")
                    from arguments in Rec(() => Expression).Separated(EntrySeparator)
                    from closeParenthesis in Token(")")
                    select new FunctionInvocationExpression() {
                        FunctionIdentifier = identifier,
                        Arguments = [..arguments],
                        SourceSpan = identifier.SourceSpan - closeParenthesis.SourceSpan
                    }
                );
            
            ArrayProtoExpression
                = Token("[]")
                    .Select<Func<Expression, Expression>>(
                        bracketPair => left => new ArrayExpression() {
                            Expression = left,
                            SourceSpan = left.SourceSpan - bracketPair.SourceSpan
                        });
            
            IntersectionProtoExpression
                = Token("&")
                    .Select<Func<Expression, Expression, Expression>>(
                        _ => (left, right) => new IntersectionExpression() {
                            Left = left,
                            Right = right,
                            SourceSpan = left.SourceSpan - right.SourceSpan
                        });
            
            UnionProtoExpression
                = Token("|")
                    .Select<Func<Expression, Expression, Expression>>(
                        _ => (left, right) => new UnionExpression() {
                            Left = left,
                            Right = right,
                            SourceSpan = left.SourceSpan - right.SourceSpan
                        });

            Expression
                = ExpressionParser.Build<char, Expression>(
                    _ => OneOf(
                        Try(FunctionInvocationExpression.Cast<Expression>()),
                        Try(IdentifierExpression.Cast<Expression>()),
                        Try(SchemaExpression.Cast<Expression>()),
                        Try(SumExpression.Cast<Expression>()),
                        Try(ParenthesizedExpression.Cast<Expression>())
                    ),
                    [
                        Operator.Postfix(Try(ArrayProtoExpression)),
                        Operator.InfixL(Try(IntersectionProtoExpression)),
                        Operator.InfixL(Try(UnionProtoExpression))
                    ]
                );
        }
    }
}