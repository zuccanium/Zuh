using System.Text;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Visitors {
    // i know most professional projects use manual traversal with the double dispatch pattern,
    // and while my first instinct was to use generic traversal, i tried to do it the professional way.
    // i really did
    // but it ended up just not ever working the way i wanted it to
    // by the end of that, every bone in my body was telling me to use generic traversal
    // so i did
    // and this is the result
    // i personally think this is way better than manual traversal
    // idk what those professionals are on
    public abstract class Visitor {
        protected abstract class Overload {
            public abstract Type Type { get; }
            public abstract Action<IZuhNode, Action> Delegate { get; }

            public override string ToString()
                => $"{nameof(Overload)} on type {Type} with delegate {Delegate}";
        }

        protected class Overload<T>(Action<T, Action> action) : Overload where T : class, IZuhNode {
            public override Type Type
                => typeof(T);

            public override Action<IZuhNode, Action> Delegate
                => (node, next) => {
                    action((T)node, next);
                };
        }
        
        protected abstract List<Overload> Overloads { get; }

        public void Visit(IZuhNode node) {
            var applicableOverloads = Overloads
                .Where(overload => node.GetType().IsAssignableTo(overload.Type));
            
            var index = 0;
            
            void callNext() {
                // go through all the wrappers
                if(applicableOverloads.ElementAtOrDefault(index++) is { } overload)
                    overload.Delegate(node, callNext);

                // do the traversal logic now
                else {
                    var enumerator = node.GetChildrenEnumerator();

                    while(enumerator.MoveNext())
                        Visit(enumerator.Current);
                }
            }

            callNext();
        }
    }
}