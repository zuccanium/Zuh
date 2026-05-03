using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, ExpressionKey> ExpressionKey = null!;
        internal static Parser<char, DynamicKey> DynamicKey = null!;
        internal static Parser<char, StaticKey> StaticKey = null!;
        internal static Parser<char, Key> Key = null!;

        private static void initializeDefinitionsKey() {
            ExpressionKey
                = CreateSchemaEntryKey(
                    CreateSchemaEntryDynamicKey(
                        WithLocation(
                            Rec(() => Expression)
                                .Select(expression => new ExpressionKey() {
                                    Expression = expression
                                })
                        )
                    )
                );

            DynamicKey
                = OneOf(
                    ExpressionKey.Cast<DynamicKey>()
                );
            
            StaticKey
                = CreateSchemaEntryKey(
                    Label.Select(label => new StaticKey() {
                        Name = label
                    })
                );

            Key
                = OneOf(
                    Try(DynamicKey.Cast<Key>()),
                    Try(StaticKey.Cast<Key>())
                );
        }
    }
}