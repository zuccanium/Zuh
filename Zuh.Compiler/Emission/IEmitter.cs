using Zuh.Compiler.Generation.Nodes;

namespace Zuh.Compiler.Emission {
    public interface IEmitter {
        public string Emit(MappingNode node);
    }
}