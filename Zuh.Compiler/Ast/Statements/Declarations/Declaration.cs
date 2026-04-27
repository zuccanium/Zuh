namespace Zuh.Compiler.Ast {
    public abstract record Declaration : Statement {
        public bool IsExport { get; init; }
    }
}