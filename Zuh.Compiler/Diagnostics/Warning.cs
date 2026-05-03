namespace Zuh.Compiler.Diagnostics {
    public record Warning : Diagnostic {
        public override DiagnosticSeverity Severity
            => DiagnosticSeverity.Warning;
    }
}