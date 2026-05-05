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

        internal static Parser<char, TDynamicKey> CreateSchemaEntryDynamicKey<TDynamicKey>(
            Parser<char, TDynamicKey> keyParser)
            where TDynamicKey : DynamicKey
            => (
                from openCaret in Token("<")
                from key in keyParser
                from closeCaret in Token(">")
                select key with {
                    SourceSpan = openCaret.SourceSpan - closeCaret.SourceSpan
                }
            );

        internal static Parser<char, TKey> CreateSchemaEntryKey<TKey>(
            Parser<char, TKey> keyParser)
            where TKey : Key
            => (
                from key in keyParser
                from optional in Try(Token("?").Optional())
                select key with {
                    IsOptional = optional.HasValue,
                    SourceSpan = optional.HasValue
                        ? key.SourceSpan - optional.Value.SourceSpan
                        : key.SourceSpan
                }
            );

        private static void initializeDefinitionsKey() {
            ExpressionKey
                = CreateSchemaEntryKey(
                    CreateSchemaEntryDynamicKey(
                        from expression in Rec(() => Expression)
                        select new ExpressionKey() {
                            Expression = expression
                        }
                    )
                );

            DynamicKey
                = OneOf(
                    ExpressionKey.Cast<DynamicKey>()
                );
            
            StaticKey
                = CreateSchemaEntryKey(
                    from label in Label
                    select new StaticKey() {
                        Name = label
                    }
                );

            Key
                = OneOf(
                    Try(DynamicKey.Cast<Key>()),
                    Try(StaticKey.Cast<Key>())
                );
        }
    }
}