using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, SchemaEntryExpressionKey> SchemaEntryExpressionKey = null!;
        internal static Parser<char, SchemaEntryDynamicKey> SchemaEntryDynamicKey = null!;
        internal static Parser<char, SchemaEntryStaticKey> SchemaEntryStaticKey = null!;
        internal static Parser<char, SchemaEntryKey> SchemaEntryKey = null!;
        internal static Parser<char, SchemaEntry> SchemaEntry = null!;
        internal static Parser<char, Schema> Schema = null!;

        internal static Parser<char, TDynamicKey> CreateSchemaEntryDynamicKey<TDynamicKey>(
            Parser<char, TDynamicKey> parser
        ) where TDynamicKey : SchemaEntryDynamicKey
            => parser
                .Between(
                    Token("<"),
                    Token(">")
                );

        internal static Parser<char, TKey> CreateSchemaEntryKey<TKey>(
            Parser<char, TKey> parser
        ) where TKey : SchemaEntryKey
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
            SchemaEntryExpressionKey
                = CreateSchemaEntryKey(
                    CreateSchemaEntryDynamicKey(
                        WithLocation(
                            Rec(() => Expression)
                                .Select(expression => new SchemaEntryExpressionKey() {
                                    Expression = expression
                                })
                        )
                    )
                );

            SchemaEntryDynamicKey
                = OneOf(
                    SchemaEntryExpressionKey.Cast<SchemaEntryDynamicKey>()
                );
            
            SchemaEntryStaticKey
                = CreateSchemaEntryKey(
                    Label.Select(label => new SchemaEntryStaticKey() {
                        Name = label
                    })
                );

            SchemaEntryKey
                = OneOf(
                    Try(SchemaEntryDynamicKey.Cast<SchemaEntryKey>()),
                    Try(SchemaEntryStaticKey.Cast<SchemaEntryKey>())
                );
            
            SchemaEntry
                = WithLocation(
                    Map(
                        (key, value) => new SchemaEntry() {
                            Key = key,
                            Value = value.GetValueOrDefault()
                        },
                        SchemaEntryKey,
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