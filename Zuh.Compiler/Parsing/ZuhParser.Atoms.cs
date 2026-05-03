using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, Identifier> Identifier = null!;
        internal static Parser<char, Label> Label = null!;

        private static Parser<char, T> createIdentifierLike<T>(Func<string, T> selector)
            where T : ZuhNode
            => WithLocation(
                Token(Map(
                        (firstLetter, remainingCharacters) => firstLetter + remainingCharacters,
                        Letter,
                        LetterOrDigit.ManyString()
                    ))
                    .Select(selector)
            );

        private static void initializeAtoms() {
            Identifier
                = createIdentifierLike(name => new Identifier() {
                    Value = name
                });
            
            Label
                = createIdentifierLike(name => new Label() {
                    Value = name
                });

            initializeAtomsLiterals();
        }
    }
}