namespace Zuh.Compiler.Semantics.Symbols {
    public abstract record ExportableSymbol : Symbol {
        public bool IsExport { get; init; } = false;
    }
}