using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, Identifier> Identifier = null!;
        internal static Parser<char, Label> Label = null!;

        private static Parser<char, T> createIdentifierLike<T>(Func<string, SourceSpan, T> selector)
            => Token(
                from firstLetter in Letter.Or(Char('_'))
                from remainingLetters in LetterOrDigit.Or(Char('_')).ManyString()
                select (firstLetter + remainingLetters)
            )
                .Select(tokenAndSource => selector(tokenAndSource.Token, tokenAndSource.SourceSpan));

        private static void initializeAtoms() {
            Identifier
                = createIdentifierLike((value, sourceSpan) => new Identifier() {
                    Value = value,
                    SourceSpan = sourceSpan,
                });
            
            Label
                = createIdentifierLike((value, sourceSpan) => new Label() {
                    Value = value,
                    SourceSpan = sourceSpan,
                });

            initializeAtomsLiterals();
        }
    }
}