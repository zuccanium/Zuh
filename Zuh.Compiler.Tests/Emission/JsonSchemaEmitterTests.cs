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
                
                Assert.Equivalent(expected, emitted);
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
        
        public class SumValue : TestSet {
            protected override MappingNode Root
                => new MappingNode() {
                    ["key"] = new MappingNode.Value() {
                        Node = new SumNode() {
                            ["a"] = new SumNode.Value(),
                            ["b"] = new SumNode.Value()
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
                      "type": "string",
                      "oneOf": [
                        {
                          "const": "a"
                        },
                        {
                          "const": "b"
                        }
                      ]
                    }
                  },
                  "required": [
                    "key"
                  ]
                }
                """;
        }

        public class SumValueWithOptionalKey : TestSet {
            protected override MappingNode Root
                => new MappingNode() {
                    ["key"] = new MappingNode.Value() {
                        Node = new SumNode() {
                            ["a"] = new SumNode.Value(),
                            ["b"] = new SumNode.Value() {
                                IsOptional = true
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
                      "type": "string",
                      "oneOf": [
                        {
                          "const": "a"
                        },
                        {
                          "const": "b"
                        }
                      ]
                    }
                  },
                  "required": [
                    "key"
                  ]
                }
                """;
        }

        public class SingleLineMappingNodeValueDocumentation : TestSet {
            protected override MappingNode Root
                => new MappingNode() {
                    ["key"] = new MappingNode.Value() {
                        Documentation = ["hi"],
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
                         "description": "hi",
                         "type": "string"
                       }
                     },
                     "required": [
                       "key"
                     ]
                   }
                   """;
        }

        public class MultiLineMappingNodeValueDocumentation : TestSet {
            protected override MappingNode Root
                => new MappingNode() {
                    ["key"] = new MappingNode.Value() {
                        Documentation = ["hi", "bye"],
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
                         "description": "hi\\nbye",
                         "type": "string"
                       }
                     },
                     "required": [
                       "key"
                     ]
                   }
                   """;
        }
        
        public class SingleLineSumNodeValueDocumentation : TestSet {
            protected override MappingNode Root
                => new MappingNode() {
                    ["key"] = new MappingNode.Value() {
                        Node = new SumNode() {
                            ["buh"] = new SumNode.Value() {
                                Documentation = ["hi"]
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
                         "type": "string",
                         "oneOf": [
                           {
                             "description": "hi",
                             "const": "buh"
                           }
                         ]
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