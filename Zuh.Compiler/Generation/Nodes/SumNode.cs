namespace Zuh.Compiler.Generation.Nodes {
    /// <summary>
    /// node that represents a sum.
    /// </summary>
    public class SumNode : Dictionary<string, SumNode.Value>, INode {
        /// <summary>
        /// encapsulates whether a key is optional.
        /// </summary>
        public class Value {
            public bool IsOptional { get; set; } = false;
            public string[]? Documentation { get; set; }
        }
        
        public SumNode() { }
        
        public SumNode(IEnumerable<KeyValuePair<string, Value>> collection)
            : base(collection) { }
    }
}