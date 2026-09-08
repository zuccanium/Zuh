using Pidgin;
using static Pidgin.Parser;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, SumEntry> SumEntry = null!;
        internal static Parser<char, Sum> Sum = null!;

        private static void initializeDefinitionsSum() {
            SumEntry
                = WithDocumentation(
                    from key in Key
                    select new SumEntry() {
                        Key = key,
                        SourceSpan = key.SourceSpan
                    }
                );
            
            Sum
                = (
                    from openBracket in Token("[")
                    from entries in SumEntry.Separated(Try(EntrySeparator))
                    from closeBracket in Token("]")
                    select new Sum() {
                        Entries = [..entries],
                        SourceSpan = openBracket.SourceSpan - closeBracket.SourceSpan
                    }
                );
        }
    }
}