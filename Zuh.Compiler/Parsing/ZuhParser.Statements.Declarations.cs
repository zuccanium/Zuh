using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Parsing {
    public static partial class ZuhParser {
        internal static Parser<char, FunctionDeclaration> FunctionDeclaration = null!;
        internal static Parser<char, SchemaDeclaration> SchemaDeclaration = null!;
        internal static Parser<char, KeysDeclaration> KeysDeclaration = null!;
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
            Func<Identifier, TWrapped, TDeclaration> func
        ) where TWrapped : ZuhNode where TDeclaration : Declaration
            => WithLocation(
                Map(
                    func,
                    Identifier,
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
            
            SchemaDeclaration
                = CreateDeclaration(
                    CreateDeclarationWrapped(
                        Schema,
                        (name, schema) => new SchemaDeclaration() {
                            Name = name,
                            Schema = schema
                        }
                    )
                );
            
            KeysDeclaration
                = CreateDeclaration(
                    CreateDeclarationWrapped(
                        Keys,
                        (name, keys) => new KeysDeclaration() {
                            Name = name,
                            Keys = keys
                        }
                    )
                );

            Declaration
                = OneOf(
                    Try(FunctionDeclaration.Cast<Declaration>()),
                    Try(SchemaDeclaration.Cast<Declaration>()),
                    Try(KeysDeclaration.Cast<Declaration>())
                );
        }
    }
}