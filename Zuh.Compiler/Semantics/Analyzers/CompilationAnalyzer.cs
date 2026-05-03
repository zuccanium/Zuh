using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;

namespace Zuh.Compiler.Semantics.Analyzers {
    public class CompilationAnalyzer : Analyzer {
        public required IImportHandler ImportHandler { get; init; }
        public required Dictionary<string, ZuhFile> Files { get; init; }

        public Dictionary<string, UnitAnalyzer> Analyzers { get; private init; } = [];
        
        public override void Analyze() {
            foreach(var (fileName, file) in Files) {
                var unitAnalyzer = new UnitAnalyzer() {
                    File = file,
                    UnitId = fileName,
                    CompilationAnalyzer = this
                };
                
                Analyzers[fileName] = unitAnalyzer;
                
                unitAnalyzer.Analyze();
            }
        }

        public Result<UnitAnalyzer, ResolutionError> ImportUnitAnalyzer(string sourceId, string module) {
            var importResolution = ImportHandler.ResolveModule(sourceId, module);

            if(importResolution is { Success: false })
                return new Result<UnitAnalyzer, ResolutionError> {
                    Diagnostic = new ResolutionError() {
                        Name = module
                    }
                };

            var importedFile = ImportHandler.FetchContent(importResolution);

            var analyzer = new UnitAnalyzer() {
                CompilationAnalyzer = this,
                File = importedFile,
                UnitId = importResolution.Id!
            };
            
            Analyzers[importResolution.Id!] = analyzer;
            
            analyzer.Analyze();

            return new Result<UnitAnalyzer, ResolutionError>() {
                Value = analyzer
            };
        }
    }
}