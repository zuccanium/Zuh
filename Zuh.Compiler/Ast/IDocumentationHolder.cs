using System.Collections.Immutable;
using System.Text;

namespace Zuh.Compiler.Ast {
    public interface IDocumentationHolder {
        public ImmutableArray<DocumentationLine>? DocumentationLines { get; init; }

        public string[]? FormattedLines {
            get {
                if(DocumentationLines is not { } actualDocumentationLines)
                    return null;

                var arr = new string[actualDocumentationLines.Length];
                var i = 0;

                foreach (var line in DocumentationLines) {
                    arr[i] = line.Value.Trim();
                    
                    i++;
                }

                return arr;
            }
        }
    }
}