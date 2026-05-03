using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Visitors {
    /// <summary>
    /// serves two functions:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             creates scopes on nodes marked as <see cref="IHasScope"/>
    ///             and adds them to <see cref="ScopeTracker.NodeToPersonalScope"/>.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             maps nodes marked as <see cref="IExistsInScope"/> to their enclosing scope
    ///             and adds them to <see cref="ScopeTracker.NodeToEnclosingScope"/>
    ///         </description>
    ///     </item>
    /// </list>
    /// </summary>
    public class ScopeCreatorVisitor : Visitor {
        private readonly Stack<Scope> scopeStack = [];
        
        public required ScopeTracker ScopeTracker { get; init; }

        private Scope? topScope
            => scopeStack.TryPeek(out var top)
                ? top
                : null;

        protected override List<Overload> Overloads
            => [
                new Overload<IHasScope>((node, next) => {
                    pushStack(node);

                    next();
            
                    popStack();
                }),
                new Overload<IExistsInScope>((node, next) => {
                    ScopeTracker.NodeToEnclosingScope[node] = topScope!;

                    next();
                })
            ];

        private void pushStack(IHasScope scopeNode) {
            var newTopScope = new Scope() {
                Parent = topScope
            };
            
            scopeStack.Push(newTopScope);
            
            ScopeTracker.NodeToPersonalScope[scopeNode] = newTopScope;
        }

        private void popStack() {
            scopeStack.Pop();
        }
    }
}