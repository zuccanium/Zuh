using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Expressions {
    public class BinaryExpressionTests {
        public static void Parse(
            string operatorString,
            out Func<Expression> leftGetter,
            out Func<Expression> rightGetter,
            out SpanMarker binaryExpressionMarker,
            out Result<char, Expression> result
        ) {
            var left = CreateExpression(out leftGetter);
            var right = CreateExpression(out rightGetter);
            
            Resolve(Mark(out binaryExpressionMarker, $"{left} {operatorString} {right}"));

            result = ZuhParser.Expression.Parse(binaryExpressionMarker.Value);
        }

        public static void AddFailingCases(string operatorString, Action<string> add) {
            {
                Resolve(Mark(out var intersectionExpression, $"{CreateExpression(out _)} {operatorString}"));
                add(intersectionExpression.Value);
            }
            {
                Resolve(Mark(out var intersectionExpression, $"{operatorString}"));
                add(intersectionExpression.Value);
            }
            {
                Resolve(Mark(out var intersectionExpression, $"{operatorString} {CreateExpression(out _)}"));
                add(intersectionExpression.Value);
            }
        }
    }
}