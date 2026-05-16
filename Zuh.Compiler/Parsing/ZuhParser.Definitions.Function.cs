using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, Function> Function = null!;
        internal static Parser<char, FunctionParameter> FunctionParameter = null!;

        private static void initializeDefinitionsFunction() {
            FunctionParameter
                = (
                    from name in Label
                    from type in LowerEnum<FunctionParameter.FunctionParameterType>()
                    select new FunctionParameter() {
                        Name = name,
                        Type = type.Token,
                        SourceSpan = name.SourceSpan - type.SourceSpan
                    }
                );

            Function
                = (
                    from openParenthesis in Token("(")
                    from parameters in FunctionParameter.Separated(Try(EntrySeparator))
                    from closeParenthesis in Token(")")
                    from expression in Rec(() => Expression)
                    select new Function() {
                        Parameters = [..parameters],
                        Expression = expression,
                        SourceSpan = openParenthesis.SourceSpan - expression.SourceSpan
                    }
                );
        }
    }
}