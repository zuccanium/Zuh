namespace Zuh.Compiler.Diagnostics {
    /// <summary>
    /// represents any issue (warning, error, maybe more later idk) during the compilation process.
    /// </summary>
    public abstract record Diagnostic {
        public enum DiagnosticSeverity {
            Warning,
            Error
        };
        
        public abstract DiagnosticSeverity Severity { get; }
    }
}