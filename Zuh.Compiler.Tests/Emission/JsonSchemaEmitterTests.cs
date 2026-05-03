using Zuh.Compiler.Emission;
using Zuh.Compiler.Generation.Nodes;

namespace Zuh.Compiler.Tests.Emission {
    public class JsonSchemaEmitterTests {
        public abstract class TestSet {
            protected abstract MappingNode Root { get; }
            protected abstract string SchemaResult { get; }

            [Fact]
            public void Works() {
                var emitter = new JsonSchemaEmitter();

                var expected = SchemaResult;
                var emitted = emitter.Emit(Root);
                
                Assert.Equivalent(emitted, expected);
            }
        }

        public class BasicSchema : TestSet {
            protected override MappingNode Root
                => new MappingNode() {
                    ["key"] = new MappingNode.Value() {
                        Node = new ScalarNode()
                    }
                };
            
            protected override string SchemaResult
                // language=json
                => """
                {
                  "type": "object",
                  "properties": {
                    "key": {
                      "type": "string"
                    }
                  },
                  "required": [
                    "key"
                  ]
                }
                """;
        }
        
        public class OptionalKey : TestSet {
            protected override MappingNode Root
                => new MappingNode() {
                    ["key"] = new MappingNode.Value() {
                        IsOptional = true,
                        Node = new ScalarNode()
                    }
                };
            
            protected override string SchemaResult
                // language=json
                => """
                {
                  "type": "object",
                  "properties": {
                    "key": {
                      "type": "string"
                    }
                  }
                }
                """;
        }
        
        public class ArrayValue : TestSet {
            protected override MappingNode Root
                => new MappingNode() {
                    ["key"] = new MappingNode.Value() {
                        Node = new ArrayNode() {
                            Node = new MappingNode() {
                                ["a"] = new MappingNode.Value() {
                                    Node = new ScalarNode()
                                }
                            }
                        }
                    }
                };
            
            protected override string SchemaResult
                // language=json
                => """
                {
                  "type": "object",
                  "properties": {
                    "key": {
                      "type": "array",
                      "items": {
                        "type": "object",
                        "properties": {
                          "a": {
                            "type": "string"
                          }
                        },
                        "required": [
                          "a"
                        ]
                      }
                    }
                  },
                  "required": [
                    "key"
                  ]
                }
                """;
        }
    }
}