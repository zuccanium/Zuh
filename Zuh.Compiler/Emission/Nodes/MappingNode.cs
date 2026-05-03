using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Zuh.Compiler.Emission.Nodes {
    public class MappingNode : Dictionary<string, MappingNode.Value>, INode {
        public class Value {
            public required INode Node { get; init; }
            public bool IsOptional { get; init; } = false;
        }
        
        public MappingNode() { }
        
        public MappingNode(IEnumerable<KeyValuePair<string, Value>> collection)
            : base(collection) { }
    }
}