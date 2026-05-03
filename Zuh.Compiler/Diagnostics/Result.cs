namespace Zuh.Compiler.Diagnostics {
    /// <summary>
    /// 🦀
    /// </summary>
    public record Result<TDiagnostic> where TDiagnostic : Diagnostic {
        public TDiagnostic? Diagnostic { get; init; }
    }
    
    public record Result<TValue, TDiagnostic> : Result<TDiagnostic> where TDiagnostic : Diagnostic {
        public TValue? Value { get; init; }
    }
}