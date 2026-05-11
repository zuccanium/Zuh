using Zuh.Compiler.Ast;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public static StaticKey StaticKeyPlaceholder
            => new() { Name = LabelPlaceholder };
        
        public static ExpressionKey ExpressionKeyPlaceholder
            => new() { Expression = ExpressionPlaceholder};

        public static Key KeyPlaceholder
            => StaticKeyPlaceholder;
        
        public static MappingNode CreateStaticKey(out Func<StaticKey> getter, StaticKey value) {
            var node = Mark(out var staticKeyMarker, $"{CreateLabel(out var labelGetter, value.Name.Value)}{(value.IsOptional ? "?" : "")}");

            getter = () => new StaticKey() {
                IsOptional = value.IsOptional,
                Name = labelGetter(),
                SourceSpan = staticKeyMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateStaticKey(out Func<StaticKey> getter, string name, bool isOptional)
            => CreateStaticKey(out getter, new StaticKey() {
                Name = new Label() {
                    Value = name
                },
                IsOptional = isOptional
            });
        
        public static MappingNode CreateStaticKey(out Func<StaticKey> getter)
            => CreateStaticKey(out getter, StaticKeyPlaceholder);

        public static MappingNode CreateExpressionKey(out Func<ExpressionKey> getter, ExpressionKey value) {
            var expressionNode = CreateExpression(out var expressionGetter, value.Expression);
            var node = Mark(out var expressionKeyMarker, $"<{expressionNode}>");

            getter = () => new ExpressionKey() {
                Expression = expressionGetter(),
                SourceSpan = expressionKeyMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateExpressionKey(out Func<ExpressionKey> getter)
            => CreateExpressionKey(out getter, ExpressionKeyPlaceholder);

        public static MappingNode CreateKey(out Func<Key> getter, Key key) {
            (MappingNode node, Func<Key> getter) createKeyTuple<TNode>(
                NodeCreator<TNode> creator,
                TNode value
            ) where TNode : Key
                => createTuple<Key, TNode>(creator, value);
            
            var nodeAndGetterTuple = key switch {
                StaticKey staticKey
                    => createKeyTuple(CreateStaticKey, staticKey),
                
                ExpressionKey expressionKey
                    => createKeyTuple(CreateExpressionKey, expressionKey),
                    
                _ => throw new NotImplementedException()
            };

            getter = nodeAndGetterTuple.getter;

            return nodeAndGetterTuple.node;
        }

        public static MappingNode CreateKey(out Func<Key> getter)
            => CreateKey(out getter, KeyPlaceholder);
    }
}