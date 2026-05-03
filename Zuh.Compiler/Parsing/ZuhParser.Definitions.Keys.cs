using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, KeysEntry> KeysEntry = null!;
        internal static Parser<char, Keys> Keys = null!;

        private static void initializeDefinitionsKeys() {
            KeysEntry
                = WithLocation(
                    Label.Select(label => new KeysEntry() {
                        Name = label
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