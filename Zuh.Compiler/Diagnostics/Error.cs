namespace Zuh.Compiler.Diagnostics {
    public record Error : Diagnostic {
        public override DiagnosticSeverity Severity
            => DiagnosticSeverity.Error;
    }
}