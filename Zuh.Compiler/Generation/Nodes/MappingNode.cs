using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Zuh.Compiler.Generation.Nodes {
    /// <summary>
    /// node that represents a mapping (schema/branch/whatever else you wanna call it). its literally just a
    /// <see cref="Dictionary{string, Value}"/> under a different name lmao.
    /// </summary>
    public class MappingNode : Dictionary<string, MappingNode.Value>, INode {
        /// <summary>
        /// encapsulates a node and whether its optional.
        /// </summary>
        public class Value {
            public required INode Node { get; init; }
            public bool IsOptional { get; init; } = false;
        }
        
        public MappingNode() { }
        
        public MappingNode(IEnumerable<KeyValuePair<string, Value>> collection)
            : base(collection) { }
    }
}