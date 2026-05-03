namespace Zuh.Compiler.Generation.Nodes {
    /// <summary>
    /// node that represents a sum.
    /// </summary>
    public class SumNode : List<SumNode.Value>, INode {
        /// <summary>
        /// encapsulates a key and whether its optional.
        /// </summary>
        public class Value {
            public required string Key { get; set; }
            public bool IsOptional { get; set; } = false;
        }
        
        public SumNode() { }
        
        public SumNode(IEnumerable<Value> collection)
            : base(collection) { }
    }
}