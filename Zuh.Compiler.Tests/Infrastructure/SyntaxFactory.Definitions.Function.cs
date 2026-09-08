using Zuh.Compiler.Ast;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public static FunctionParameter FunctionParameterPlaceholder
            => new() {
                Name = LabelPlaceholder,
                Type = FunctionParameter.FunctionParameterType.Schema
            };

        public static Function FunctionPlaceholder
            => new() {
                Parameters = [],
                Expression = ExpressionPlaceholder
            };
        
        public static MappingNode CreateFunctionParameter(out Func<FunctionParameter> getter, FunctionParameter value) {
            var node = Mark(out var functionParameterMarker, $"{Mark(out var nameMarker, value.Name.Value)} {value.Type.ToString().ToLowerInvariant()}");

            getter = () => new FunctionParameter() {
                Name = new Label() {
                    Value = nameMarker.Value,
                    SourceSpan = nameMarker.SourceSpan
                },
                Type = FunctionParameter.FunctionParameterType.Schema,
                SourceSpan = functionParameterMarker.SourceSpan
            };

            return node;
        }
        
        public static MappingNode CreateFunctionParameter(out Func<FunctionParameter> getter)
            => CreateFunctionParameter(out getter, FunctionParameterPlaceholder);
        
        public static MappingNode CreateFunction(out Func<Function> getter, Function value) {
            var parameterNodes = value.Parameters
                .SelectWithOut(
                    out var parameterGetters,
                    (FunctionParameter parameter, out Func<FunctionParameter> outValue)
                        => CreateFunctionParameter(out outValue, parameter)
                );
            
            var node = Mark(out var functionMarker, $"({parameterNodes.MarkAsJoined(", ")}) {CreateExpression(out var expressionGetter, value.Expression)}");

            getter = () => new Function() {
                Parameters = [
                    ..parameterGetters
                        .Select(getter => getter())
                ],
                Expression = expressionGetter(),
                SourceSpan = functionMarker.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateFunction(out Func<Function> getter)
            => CreateFunction(out getter, FunctionPlaceholder);
    }
}