using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, SchemaEntry> SchemaEntry = null!;
        internal static Parser<char, Schema> Schema = null!;

        internal static Parser<char, TDynamicKey> CreateSchemaEntryDynamicKey<TDynamicKey>(
            Parser<char, TDynamicKey> parser
        ) where TDynamicKey : DynamicKey
            => parser
                .Between(
                    Token("<"),
                    Token(">")
                );

        internal static Parser<char, TKey> CreateSchemaEntryKey<TKey>(
            Parser<char, TKey> parser
        ) where TKey : Key
            => WithLocation(
                Map(
                    (key, optional) => key with {
                        IsOptional = optional.HasValue
                    },
                    parser,
                    Try(Token("?").Optional())
                )
            );

        private static void initializeDefinitionsSchema() {
            SchemaEntry
                = WithLocation(
                    Map(
                        (key, value) => new SchemaEntry() {
                            Key = key,
                            Value = value.GetValueOrDefault()
                        },
                        Key,
                        Rec(() => Expression!).Optional()
                    )
                );
            
            Schema
                = WithLocation(
                    SchemaEntry
                        .Separated(EntrySeparator)
                        .Between(
                            Token("{"),
                            Token("}")
                        )
                        .Select(entries => new Schema() {
                            Entries = [..entries]
                        })
                );
        }
    }
}