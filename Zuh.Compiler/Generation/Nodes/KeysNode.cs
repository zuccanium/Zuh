namespace Zuh.Compiler.Generation.Nodes {
    public class KeysNode : List<KeysNode.Value>, INode {
        public class Value {
            public required string Key { get; set; }
            public bool IsOptional { get; set; } = false;
        }
        
        public KeysNode() { }
        
        public KeysNode(IEnumerable<Value> collection)
            : base(collection) { }
    }
}