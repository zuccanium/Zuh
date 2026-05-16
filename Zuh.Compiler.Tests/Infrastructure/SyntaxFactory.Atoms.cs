using Zuh.Compiler.Ast;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public static Identifier IdentifierPlaceholder
            => new() { Value = Placeholder };
        
        public static Label LabelPlaceholder
            => new() { Value = Placeholder };
        
        private static MappingNode createIdentifierLike<TIdentifierLike>(
            out Func<TIdentifierLike> getter,
            TIdentifierLike value,
            string valueValue
        ) where TIdentifierLike : ZuhNode {
            var node = Mark(out var valueMarker, valueValue);

            getter = () => value with {
                SourceSpan = valueMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateIdentifier(out Func<Identifier> getter, Identifier value)
            => createIdentifierLike(out getter, value, value.Value);
        
        public static MappingNode CreateIdentifier(out Func<Identifier> getter, string value)
            => CreateIdentifier(out getter, new Identifier() {
                Value = value
            });
        
        public static MappingNode CreateIdentifier(out Func<Identifier> getter)
            => CreateIdentifier(out getter, IdentifierPlaceholder);
        
        public static MappingNode CreateLabel(out Func<Label> getter, Label value)
            => createIdentifierLike(out getter, value, value.Value);
        
        public static MappingNode CreateLabel(out Func<Label> getter, string value)
            => CreateLabel(out getter, new Label() {
                Value = value
            });
        
        public static MappingNode CreateLabel(out Func<Label> getter)
            => CreateLabel(out getter, LabelPlaceholder);
    }
}