using Zuh.Compiler.Ast;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public static SumEntry SumEntryPlaceholder
            => new() {
                Key = KeyPlaceholder,
                DocumentationLines = []
            };

        public static Sum SumPlaceholder
            => new() {
                Entries = []
            };
        
        public static MappingNode CreateSumEntry(out Func<SumEntry> getter, SumEntry value) {
            var node = Mark(out var sumEntryMarker, $"{CreateKey(out var keyGetter, value.Key)}");

            getter = () => new SumEntry() {
                Key = keyGetter(),
                DocumentationLines = [],
                SourceSpan = sumEntryMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateSumEntry(out Func<SumEntry> getter)
            => CreateSumEntry(out getter, SumEntryPlaceholder);

        public static MappingNode CreateSum(out Func<Sum> getter, Sum value) {
            var entryNodes = value.Entries.SelectWithOut(
                out var entryGetters,
                (SumEntry source, out Func<SumEntry> outValue) => CreateSumEntry(out outValue, source)
            );

            var node = Mark(out var sumMarker, $"[ {entryNodes.MarkAsJoined(", ")} ]");

            getter = () => new Sum() {
                Entries = [
                    ..entryGetters
                        .Select(getter => getter())
                ],
                SourceSpan = sumMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateSum(out Func<Sum> getter)
            => CreateSum(out getter, SumPlaceholder);
    }
}