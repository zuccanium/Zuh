using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal record struct TokenAndSource<T>(T Token, SourceSpan SourceSpan);
        
        internal static readonly Parser<char, string> SingleLineComment;
        internal static readonly Parser<char, string> MultiLineComment;
        internal static readonly Parser<char, IEnumerable<string>> Trivia;
        internal static readonly Parser<char, Unit> EntrySeparator;
        internal static readonly Parser<char, ZuhFile> ZuhFile;

        internal static Parser<char, TokenAndSource<T>> Token<T>(Parser<char, T> parser)
            => SkipWhitespaces
                .Then(
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

        internal static Parser<char, T> WithLocation<T>(Parser<char, T> parser) where T : ZuhNode
            => Map(
                (start, node, end)
                    => node with {
                        SourceSpan = new SourceSpan() {
                            Start = start,
                            End = end
                        }
                    },
                CurrentOffset,
                parser,
                CurrentOffset
            );
        
        internal static Parser<char, T> WithTrivia<T>(Parser<char, T> parser) where T : ZuhNode, ITriviaHolder
            => Map(
                (trivia, triviaHolder) => triviaHolder with {
                    TriviaLines = [..trivia.Value]
                },
                Trivia.Optional(),
                parser
            );

        static ZuhParser() {
            SingleLineComment
                = Try(String("//"))
                    .Then(AnyCharExcept('\n', '\r').ManyString());
            
            MultiLineComment
                = Try(String("/*"))
                    .Then(
                        Any
                            .Until(Try(String("*/")))
                            .Select(chars => new string(chars.ToArray()))
                    );
            
            Trivia
                = OneOf(
                    SingleLineComment.Select(str => str.Trim()),
                    MultiLineComment.Select(str => str.Trim()),
                    Whitespace.AtLeastOnce().ThenReturn("")
                )
                    .Many()
                    .Select(results => results.Where(str => !string.IsNullOrWhiteSpace(str)));

            EntrySeparator
                = Token(",")
                    .ThenReturn(Unit.Value);
            
            initializeAtoms();
            initializeDefinitions();
            initializeExpressions();
            initializeStatements();
            
            ZuhFile
                = WithLocation(
                    SkipWhitespaces
                        .Then(
                            Statement
                                .Many()
                                .Select(statements => new ZuhFile() {
                                    RootStatements = [..statements]
                                })
                        )
                );
        }

        public static ZuhFile ParseOrThrow(string input)
            => ZuhFile.ParseOrThrow(input);
    }
}