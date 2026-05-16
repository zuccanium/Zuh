namespace Zuh.Compiler.Diagnostics {
    public abstract record Error : Diagnostic {
        public override DiagnosticSeverity Severity
            => DiagnosticSeverity.Error;
    }
}