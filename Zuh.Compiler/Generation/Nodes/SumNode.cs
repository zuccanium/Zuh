namespace Zuh.Compiler.Generation.Nodes {
    /// <summary>
    /// node that represents a sum.
    /// </summary>
    /// <remarks>
    /// <b>this is only a tool used in the generation phase!!!</b>
    /// the emitter should never encounter this.
    /// </remarks>
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