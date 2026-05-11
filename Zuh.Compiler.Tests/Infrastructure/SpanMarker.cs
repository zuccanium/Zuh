using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Tests.Infrastructure.Extensions;

namespace Zuh.Compiler.Tests.Infrastructure {
    /// <summary>
    /// utility class for creating strings of zuh code with markers for each section.
    /// </summary>
    public class SpanMarker {
        public abstract class Node {
            public abstract List<Node> Children { get; }
        }

        public class StringNode : Node {
            public override List<Node> Children
                => [];

            public required string Value { get; set; }
        }
        
        public class MappingNode : Node {
            public override List<Node> Children { get; } = [];
            public required SpanMarker SpanMarker { get; set; }
        }
        
        [InterpolatedStringHandler]
        public class SpanMarkingInterpolatedStringHandler {
            public MappingNode MappingNode { get; init; } = new() {
                SpanMarker = new SpanMarker()
            };
            
            public SpanMarkingInterpolatedStringHandler(int literalLength, int formattedCount) { }

            public void AppendLiteral(string literal)
                => AppendFormatted(new StringNode() {
                    Value = literal
                });

            public void AppendFormatted<T>(T formatted) {
                if(formatted is Node formattedNode)
                    AppendFormatted(formattedNode);

                else
                    AppendLiteral(formatted?.ToString() ?? "");
            }

            public void AppendFormatted(Node node)
                => MappingNode.Children.Add(node);

            public void AppendFormatted<T>(IEnumerableExtensions.MarkedAsJoined<T> joined) {
                if(!joined.Enumerable.Any())
                    return;
                    
                foreach(var member in joined.Enumerable) {
                    AppendFormatted(member);
                    
                    AppendFormatted(new StringNode() {
                        Value = joined.Joiner
                    });
                }
                
                MappingNode.Children.RemoveAt(MappingNode.Children.Count - 1);
            }
        }
        
        public SourceSpan SourceSpan { get; set; }
        public string Value { get; set; } = null!;

        public static MappingNode Mark(out SpanMarker spanMarker, SpanMarkingInterpolatedStringHandler formatted) {
            spanMarker = formatted.MappingNode.SpanMarker;

            return formatted.MappingNode;
        }
        
        public static MappingNode Mark(out SpanMarker spanMarker, string literal) {
            var mappingNode = new MappingNode() {
                Children = {
                    new StringNode() {
                        Value = literal
                    }
                },
                SpanMarker = spanMarker = new SpanMarker()
            };

            return mappingNode;
        }

        public static void Resolve(MappingNode node) {
            StringBuilder recur(MappingNode current, int startIndex) {
                var builder = new StringBuilder();
                    
                foreach(var child in current.Children) {
                    if(child is StringNode stringChild)
                        builder.Append(stringChild.Value);

                    else if(child is MappingNode mappingChild)
                        builder.Append(recur(mappingChild, startIndex + builder.Length));
                }

                current.SpanMarker.Value = builder.ToString();
                current.SpanMarker.SourceSpan = new SourceSpan() {
                    Start = startIndex,
                    End = startIndex + builder.Length
                };

                return builder;
            }
                
            recur(node, 0);
        }
    }
}