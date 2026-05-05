using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, SchemaEntry> SchemaEntry = null!;
        internal static Parser<char, Schema> Schema = null!;

        private static void initializeDefinitionsSchema() {
            SchemaEntry
                = (
                    from key in Key
                    from value in Rec(() => Expression!).Optional()
                    select new SchemaEntry() {
                        Key = key,
                        Value = value.GetValueOrDefault(),
                        SourceSpan = value.HasValue
                            ? key.SourceSpan - value.Value.SourceSpan
                            : key.SourceSpan
                    }
                );

            Schema
                = (
                    from openBrace in Token("{")
                    from entries in SchemaEntry.Separated(EntrySeparator)
                    from closeBrace in Token("}")
                    select new Schema() {
                        Entries = [..entries],
                        SourceSpan = openBrace.SourceSpan - closeBrace.SourceSpan
                    }
                );
        }
    }
}