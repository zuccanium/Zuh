using Newtonsoft.Json.Schema;
using Zuh.Compiler.Generation.Nodes;

namespace Zuh.Compiler.Emission {
    public class JsonSchemaEmitter : IEmitter {
        public string Emit(MappingNode node)
            => mappingNodeToSchema(node).ToString();

        private JSchema nodeToSchema(INode node)
            => node switch {
                MappingNode mappingNode => mappingNodeToSchema(mappingNode),
                ArrayNode arrayNode => arrayNodeToSchema(arrayNode),
                SumNode sumNode => sumNodeToSchema(sumNode),
                ScalarNode scalarNode => scalarNodeToSchema(scalarNode),
                _ => throw new InvalidOperationException($"unexpected {nameof(INode)} inheritor!!!")
            };

        private JSchema mappingNodeToSchema(MappingNode node) {
            var schema = new JSchema() {
                Type = JSchemaType.Object
            };
            
            foreach(var (key, value) in node) {
                schema.Properties[key] = nodeToSchema(value.Node);

                if(!value.IsOptional)
                    schema.Required.Add(key);
            }

            return schema;
        }
        
        private JSchema arrayNodeToSchema(ArrayNode node) {
            var schema = new JSchema() {
                Type = JSchemaType.Array,
                Items = {
                    nodeToSchema(node.Node)
                }
            };
            
            return schema;
        }

        private JSchema sumNodeToSchema(SumNode node) {
            var schema = new JSchema() {
                Type = JSchemaType.String,
            };
            
            // please give me a setter 😭
            foreach(var (key, value) in node)
                schema.Enum.Add(key);

            return schema;
        }
        
        private JSchema scalarNodeToSchema(ScalarNode node) {
            var schema = new JSchema() {
                Type = JSchemaType.String
            };
            
            return schema;
        }
    }
}