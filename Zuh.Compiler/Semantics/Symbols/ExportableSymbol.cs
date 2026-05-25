namespace Zuh.Compiler.Semantics.Symbols {
    public abstract class ExportableSymbol : Symbol {
        public bool IsExport { get; init; } = false;
    }
}