using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, StringLiteral> StringLiteral = null!;
        internal static Parser<char, Literal> Literal = null!;

        private static void initializeAtomsLiterals() {
            StringLiteral
                = WithLocation(
                    AnyCharExcept('"')
                        .ManyString()
                        .Between(
                            Char('"'),
                            Char('"')
                        )
                        .Select(str => new StringLiteral() {
                            Value = str
                        })
                );
            
            Literal
                = OneOf(
                    StringLiteral.Cast<Literal>()
                );
        }
    }
}