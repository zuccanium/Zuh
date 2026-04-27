using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, Statement> Statement = null!;
        internal static Parser<char, ImportStatement> ImportStatement = null!;

        internal static Parser<char, TStatement> CreateStatement<TStatement>(
            Parser<char, TStatement> parser
        ) where TStatement : Statement
            => WithLocation(
                WithTrivia(
                    parser
                        .Before(Token(";"))
                )
            );
        
        private static void initializeStatements() {
            initializeStatementsDeclarations();

            ImportStatement
                = CreateStatement(
                    Token("import")
                        .Then(StringLiteral)
                        .Select(stringLiteral => new ImportStatement() {
                            Module = stringLiteral
                        })
                );

            Statement
                = OneOf(
                    Try(ImportStatement.Cast<Statement>()),
                    Try(Declaration.Cast<Statement>())
                );
        }
    }
}