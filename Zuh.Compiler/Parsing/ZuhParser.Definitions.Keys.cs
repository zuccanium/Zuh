using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, KeysEntry> KeysEntry = null!;
        internal static Parser<char, Keys> Keys = null!;

        private static void initializeDefinitionsKeys() {
            KeysEntry
                = WithLocation(
                    Identifier.Select(identifier => new KeysEntry() {
                        Identifier = identifier
                    })
                );
            
            Keys
                = WithLocation(
                    KeysEntry
                        .Between(SkipWhitespaces)
                        .Separated(EntrySeparator)
                        .Between(
                            Token("["),
                            Token("]")
                        )
                        .Select(entries => new Ast.Keys() {
                            Entries = [..entries]
                        })
                );
        }
    }
}