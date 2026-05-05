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
                = WithTrivia(
                    from key in Key
                    select new SumEntry() {
                        Key = key
                    }
                );
            
            Sum
                = (
                    from openBracket in Token("[")
                    from entries in SumEntry.Separated(EntrySeparator)
                    from closeBracket in Token("]")
                    select new Sum() {
                        Entries = [..entries],
                        SourceSpan = openBracket.SourceSpan - closeBracket.SourceSpan
                    }
                );
        }
    }
}