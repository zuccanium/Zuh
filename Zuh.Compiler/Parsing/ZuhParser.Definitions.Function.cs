using System.Collections.Immutable;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, Function> Function = null!;
        internal static Parser<char, FunctionParameter> FunctionParameter = null!;

        private static void initializeDefinitionsFunction() {
            FunctionParameter
                = Map(
                    (name, type) => new FunctionParameter() {
                        Name = name,
                        Type = type
                    },
                    Identifier,
                    LowerEnum<FunctionParameter.FunctionParameterType>()
                );

            Function
                = Map(
                    (parameters, expression) => new Function() {
                        Parameters = [..parameters],
                        Expression = expression
                    },
                    FunctionParameter
                        .Separated(EntrySeparator)
                        .Between(
                            Token("("),
                            Token(")")
                        ),
                    Rec(() => Expression)
                );
        }
    }
}