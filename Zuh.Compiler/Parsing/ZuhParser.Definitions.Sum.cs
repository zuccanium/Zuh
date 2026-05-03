using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, SumEntry> SumEntry = null!;
        internal static Parser<char, Sum> Sum = null!;

        private static void initializeDefinitionsSum() {
            SumEntry
                = WithLocation(
                    Key.Select(key => new SumEntry() {
                        Key = key
                    })
                );
            
            Sum
                = WithLocation(
                    SumEntry
                        .Between(SkipWhitespaces)
                        .Separated(EntrySeparator)
                        .Between(
                            Token("["),
                            Token("]")
                        )
                        .Select(entries => new Ast.Sum() {
                            Entries = [..entries]
                        })
                );
        }
    }
}