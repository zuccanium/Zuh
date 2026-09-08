using Pidgin;
using static Pidgin.Parser;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, StringLiteral> StringLiteral = null!;
        internal static Parser<char, Literal> Literal = null!;

        private static void initializeAtomsLiterals() {
            StringLiteral
                = (
                    from openQuote in Token(Char('"'))
                    from str in AnyCharExcept('"').ManyString()
                    from closeQuote in Token(Char('"'))
                    select new StringLiteral() {
                        Value = str,
                        SourceSpan = openQuote.SourceSpan - closeQuote.SourceSpan
                    }
                );
            
            Literal
                = OneOf(
                    StringLiteral.Cast<Literal>()
                );
        }
    }
}