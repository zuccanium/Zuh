using Zuh.Compiler.Generation.Nodes;

namespace Zuh.Compiler.Emission {
    /// <summary>
    /// thing that represents everything an emitter can do. its only job is to emit stuff given a mapping node.
    /// </summary>
    public interface IEmitter {
        public string Emit(MappingNode node);
    }
}