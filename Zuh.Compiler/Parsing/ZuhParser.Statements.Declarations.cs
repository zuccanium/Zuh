using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, FunctionDeclaration> FunctionDeclaration = null!;
        internal static Parser<char, ExpressionDeclaration> ExpressionDeclaration = null!;
        internal static Parser<char, Declaration> Declaration = null!;
        
        internal static Parser<char, TDeclaration> CreateDeclaration<TDeclaration>(
            Parser<char, TDeclaration> parser
        ) where TDeclaration : Declaration
            => CreateStatement(
                Map(
                    (export, declaration) => declaration with {
                        IsExport = export.HasValue
                    },
                    Keyword("export").Optional(),
                    parser
                )
            );
        
        // the monster
        internal static Parser<char, TDeclaration> CreateDeclarationWrapped<TWrapped, TDeclaration>(
            Parser<char, TWrapped> parser,
            Func<Label, TWrapped, TDeclaration> func
        ) where TWrapped : ZuhNode where TDeclaration : Declaration
            => WithLocation(
                Map(
                    func,
                    Label,
                    parser
                )
            );
            
        private static void initializeStatementsDeclarations() {
            FunctionDeclaration
                = CreateDeclaration(
                    CreateDeclarationWrapped(
                        Function,
                        (name, function) => new FunctionDeclaration() {
                            Name = name,
                            Function = function
                        }
                    )
                );
            
            ExpressionDeclaration
                = CreateDeclaration(
                    CreateDeclarationWrapped(
                        Expression,
                        (name, expression) => new ExpressionDeclaration() {
                            Name = name,
                            Expression = expression
                        }
                    )
                );

            Declaration
                = OneOf(
                    Try(FunctionDeclaration.Cast<Declaration>()),
                    Try(ExpressionDeclaration.Cast<Declaration>())
                );
        }
    }
}