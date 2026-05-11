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

        internal static Parser<char, TDynamicKey> CreateDynamicKey<TDynamicKey>(
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

        internal static Parser<char, TKey> CreateKey<TKey>(
            Parser<char, TKey> keyParser)
            where TKey : Key
            => (
                from key in keyParser
                from optional in Try(Token("?")).Optional()
                select key with {
                    IsOptional = optional.HasValue,
                    SourceSpan = optional.HasValue
                        ? key.SourceSpan - optional.Value.SourceSpan
                        : key.SourceSpan
                }
            );

        private static void initializeDefinitionsKey() {
            ExpressionKey
                = CreateKey(
                    CreateDynamicKey(
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
                = CreateKey(
                    from label in Label
                    select new StaticKey() {
                        Name = label,
                        SourceSpan = label.SourceSpan
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