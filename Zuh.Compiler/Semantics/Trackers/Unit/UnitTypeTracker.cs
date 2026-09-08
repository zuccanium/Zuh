using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics.Types;

namespace Zuh.Compiler.Semantics.Trackers.Unit {
    /// <summary>
    /// keeps track of type information in expressions in a <see cref="ZuhFile"/>.
    /// </summary>
    public class UnitTypeTracker {
        /// <summary>
        /// map of expressions -> the type of the expression.
        /// </summary>
        public Dictionary<Expression, ZuhType> ExpressionToType { get; set; } = [];
    }
}