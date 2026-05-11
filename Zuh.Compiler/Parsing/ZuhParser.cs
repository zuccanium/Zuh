using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal record struct TokenAndSource<T>(T Token, SourceSpan SourceSpan);
        
        internal static readonly Parser<char, Unit> CommentLine;
        internal static readonly Parser<char, Unit> EntrySeparator;
        internal static readonly Parser<char, Unit> SkipGarbage;
        internal static readonly Parser<char, DocumentationLine> DocumentationLine;
        internal static readonly Parser<char, ZuhFile> ZuhFile;

        internal static Parser<char, TokenAndSource<T>> WithSource<T>(Parser<char, T> parser)
            => (
                from start in CurrentOffset
                from token in parser
                from end in CurrentOffset
                select new TokenAndSource<T>() {
                    Token = token,
                    SourceSpan = new SourceSpan() {
                        Start = start,
                        End = end
                    }
                }
            );
        
        internal static Parser<char, TokenAndSource<T>> Token<T>(Parser<char, T> parser)
            => SkipGarbage
                .Then(WithSource(parser));

        internal static Parser<char, TokenAndSource<string>> Token(string token)
            => Token(String(token));

        internal static Parser<char, TokenAndSource<string>> Keyword(string keyword)
            => Token(String(keyword).Before(Not(LetterOrDigit)));
        
        internal static Parser<char, TokenAndSource<T>> LowerEnum<T>() where T : struct, Enum
            => Token(
                from value in OneOf(
                    System.Enum.GetNames<T>()
                        .Select(name => Try(String(name.ToLowerInvariant())))
                )
                select System.Enum.Parse<T>(value, true)
            );
        
        internal static Parser<char, T> WithDocumentation<T>(Parser<char, T> belowParser) where T : ZuhNode, IDocumentationHolder
            => (
                from documentation in Try(DocumentationLine).Many()
                from below in belowParser
                select below with {
                    DocumentationLines = [..documentation]
                }
            );

        static ZuhParser() {
            CommentLine
                = (
                    from start in SkipWhitespaces.Then(WithSource(String("//")))
                    from body in WithSource(AnyCharExcept('\n', '\r').ManyString())
                    select Unit.Value
                );

            SkipGarbage
                = OneOf(
                    Try(Whitespace).ThenReturn(Unit.Value),
                    Try(CommentLine)
                )
                    .SkipMany();

            DocumentationLine
                = (
                    from start in SkipGarbage.Then(WithSource(String("///")))
                    from body in WithSource(AnyCharExcept('\n', '\r').ManyString())
                    select new DocumentationLine() {
                        Value = body.Token,
                        SourceSpan = start.SourceSpan - body.SourceSpan
                    }
                );

            EntrySeparator
                = Token(",")
                    .ThenReturn(Unit.Value);
            
            initializeAtoms();
            initializeDefinitions();
            initializeExpressions();
            initializeStatements();

            ZuhFile
                = (
                    from statements in Statement.Many()
                    select new ZuhFile() {
                        RootStatements = [..statements],
                        SourceSpan = statements.Any()
                            ? statements.First().SourceSpan - statements.Last().SourceSpan
                            : new SourceSpan() {
                                Start = 0,
                                End = 0
                            }
                    }
                );
        }

        public static ZuhFile ParseOrThrow(string input)
            => ZuhFile.ParseOrThrow(input);
    }
}