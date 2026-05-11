using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, Statement> Statement = null!;
        internal static Parser<char, ImportStatement> ImportStatement = null!;

        internal static Parser<char, TStatement> CreateStatement<TStatement>(
            Parser<char, TStatement> statementParser)
            where TStatement : Statement
            => WithDocumentation(
                from parser in statementParser
                from semicolon in Token(";")
                select parser with {
                    SourceSpan = parser.SourceSpan - semicolon.SourceSpan,
                }
            );
        
        private static void initializeStatements() {
            initializeStatementsDeclarations();

            ImportStatement
                = CreateStatement(
                    from import in Token("import")
                    from stringLiteral in StringLiteral
                    select new ImportStatement() {
                        Module = stringLiteral,
                        SourceSpan = import.SourceSpan - stringLiteral.SourceSpan
                    }
                );

            Statement
                = OneOf(
                    Try(ImportStatement.Cast<Statement>()),
                    Try(Declaration.Cast<Statement>())
                );
        }
    }
}