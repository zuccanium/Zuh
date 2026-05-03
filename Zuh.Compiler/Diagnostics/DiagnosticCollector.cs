using System.Collections;

namespace Zuh.Compiler.Diagnostics {
    public class DiagnosticCollector : ICollection<Diagnostic> {
        private readonly List<Diagnostic> diagnostics = [];

        public void Add(Diagnostic diagnostic)
            => diagnostics.Add(diagnostic);

        public void Clear()
            => diagnostics.Clear();

        public bool Contains(Diagnostic item) => throw new NotImplementedException();

        public void CopyTo(Diagnostic[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(Diagnostic item) => throw new NotImplementedException();

        public int Count
            => diagnostics.Count;
        
        public bool IsReadOnly
            => false;

        public IEnumerator<Diagnostic> GetEnumerator()
            => diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}