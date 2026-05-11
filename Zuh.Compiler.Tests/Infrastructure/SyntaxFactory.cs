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
    }
}