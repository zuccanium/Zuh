using Zuh.Compiler.Ast;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public const string Placeholder = "idk";

        private delegate MappingNode NodeCreator<TNode>(out Func<TNode> getter, TNode value)
            where TNode : ZuhNode;
        
        private static (MappingNode node, Func<TGeneralNode> getter) createTuple<TGeneralNode, TSpecificNode>(
            NodeCreator<TSpecificNode> creator,
            TSpecificNode value
        ) where TSpecificNode : ZuhNode, TGeneralNode {
            var node = creator(out var getter, value);

            return (node, () => getter());
        }

        public static MappingNode CreateDocumentationLine(out Func<DocumentationLine> getter, string value) {
            var node = Mark(out var documentationLineMarker, $"///{value}");

            getter = () => new DocumentationLine() {
                Value = value,
                SourceSpan = documentationLineMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateDocumentationLine(out Func<DocumentationLine> getter)
            => CreateDocumentationLine(out getter, Placeholder);
    }
}