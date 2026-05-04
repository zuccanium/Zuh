namespace Zuh.Compiler.Generation.Nodes {
    /// <summary>
    /// node that represents a node that wraps another node in array. this is different from <see cref="SumNode"/>.
    /// </summary>
    public record ArrayNode : INode {
        public required INode Node { get; set; }
    }
}