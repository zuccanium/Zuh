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
            Parser<char, TDeclaration> declarationParser
        ) where TDeclaration : Declaration
            => CreateStatement(
                WithDocumentation(
                    from export in Try(Keyword("export")).Optional()
                    from declaration in declarationParser
                    select declaration with {
                        IsExport = export.HasValue,
                        SourceSpan = export.HasValue
                            ? export.Value.SourceSpan - declaration.SourceSpan
                            : declaration.SourceSpan
                    }
                )
            );
        
        // the monster
        internal static Parser<char, TDeclaration> CreateLabeledDefinition<TDefinition, TDeclaration>(
            Parser<char, TDefinition> definitionParser,
            Func<Label, TDefinition, TDeclaration> selector
        ) where TDefinition : ZuhNode where TDeclaration : Declaration
            => (
                from label in Label
                from definition in definitionParser
                select selector(label, definition) with {
                    SourceSpan = label.SourceSpan - definition.SourceSpan
                }
            );
            
        private static void initializeStatementsDeclarations() {
            FunctionDeclaration
                = CreateDeclaration(
                    CreateLabeledDefinition(
                        Function,
                        (name, function) => new FunctionDeclaration() {
                            Name = name,
                            Function = function
                        }
                    )
                );
            
            ExpressionDeclaration
                = CreateDeclaration(
                    CreateLabeledDefinition(
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