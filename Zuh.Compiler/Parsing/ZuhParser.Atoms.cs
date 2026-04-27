using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, Identifier> Identifier = null!;
        
        private static void initializeAtoms() {
            Identifier
                = WithLocation(
                    Token(Map(
                            (firstLetter, remainingCharacters) => firstLetter + remainingCharacters,
                            Letter,
                            LetterOrDigit.ManyString()
                        ))
                        .Select(name => new Identifier() { Value = name })
                );

            initializeAtomsLiterals();
        }
    }
}