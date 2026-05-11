using Zuh.Compiler.Ast;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public static SchemaEntry SchemaEntryPlaceholder
            => new() {
                Key = KeyPlaceholder,
                DocumentationLines = []
            };

        public static Schema SchemaPlaceholder
            => new() { Entries = [] };
        
        public static MappingNode CreateSchemaEntry(out Func<SchemaEntry> getter, SchemaEntry value) {
            var valueGetter = default(Func<Expression>);
            
            var keyNode = CreateKey(out var keyGetter, value.Key);
            var valueNodeMaybe = value.Value is { } existingValue
                ? CreateExpression(out valueGetter, existingValue)
                : null;

            var node = valueNodeMaybe is { } valueNode
                ? Mark(out var schemaEntryMarker, $"{keyNode} {valueNode}")
                : Mark(out schemaEntryMarker, $"{keyNode}");
                
            getter = () => new SchemaEntry() {
                Key = keyGetter(),
                Value = valueGetter?.Invoke(),
                DocumentationLines = [],
                SourceSpan = schemaEntryMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateSchemaEntry(out Func<SchemaEntry> getter)
            => CreateSchemaEntry(out getter, SchemaEntryPlaceholder);
        
        public static MappingNode CreateSchema(out Func<Schema> getter, Schema value) {
            var entryNodes = value.Entries.SelectWithOut(
                out var entryGetters,
                (SchemaEntry entry, out Func<SchemaEntry> outValue)
                    => CreateSchemaEntry(out outValue, entry)
            );
            
            var node = Mark(out var schemaMarker, $"{{ {entryNodes.MarkAsJoined(", ")} }}");

            getter = () => new Schema() {
                Entries = [
                    ..entryGetters
                        .Select(getter => getter())
                ],
                SourceSpan = schemaMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateSchema(out Func<Schema> getter)
            => CreateSchema(out getter, SchemaPlaceholder);
    }
}