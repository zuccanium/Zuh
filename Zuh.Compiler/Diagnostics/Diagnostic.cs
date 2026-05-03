namespace Zuh.Compiler.Diagnostics {
    public abstract record Diagnostic {
        public enum DiagnosticSeverity {
            Warning,
            Error
        };
        
        public abstract DiagnosticSeverity Severity { get; }
    }
}