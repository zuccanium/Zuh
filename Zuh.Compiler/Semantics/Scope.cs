using System.Collections;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Semantics {
    /// <summary>
    /// keeps track of all symbols in a scope and handles resolution between a scope and its ancestors.
    /// </summary>
    public class Scope : IEnumerable<KeyValuePair<string, Symbol>> {
        public Dictionary<string, Symbol> Symbols { get; init; } = [];
        public Scope? Parent { get; init; }

        public Result<DeclarationError> Declare(Symbol entry) {
            if(!Symbols.TryAdd(entry.Name, entry))
                return new Result<DeclarationError>() {
                    Diagnostic = new DeclarationError()
                };

            return new Result<DeclarationError>();
        }

        public Result<Symbol, ResolutionError> Resolve(string name) {
            if(Symbols.TryGetValue(name, out var symbol))
                return new Result<Symbol, ResolutionError>() {
                    Value = symbol
                };

            if(Parent is { } parent)
                return parent.Resolve(name);

            return new Result<Symbol, ResolutionError>() {
                Diagnostic = new ResolutionError() {
                    Name = name
                }
            };
        }

        public IEnumerator<KeyValuePair<string, Symbol>> GetEnumerator()
            => Symbols.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}