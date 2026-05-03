namespace Zuh.Compiler.Generation.Nodes {
    /// <summary>
    /// thing that all nodes implement.
    /// </summary>
    /// <remarks>
    /// its an interface because some nodes (e.g. <see cref="MappingNode"/>, <see cref="ArrayNode"/>) inherit from data
    /// structures to avoid making 10 billion wrapper methods.
    /// </remarks>
    public interface INode;
}