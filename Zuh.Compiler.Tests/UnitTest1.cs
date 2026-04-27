using Zuh.Compiler.Parsing;

namespace Zuh.Compiler.Tests {
    public class UnitTest1 {
        [Fact]
        public void Test1() {
            var result = ZuhParser.ParseOrThrow(
                """
                export Difficulties [
                    Easy,
                    Normal,
                    Hard,
                    Lunatic
                ];
                
                character(spellIds keys) {
                    Title,
                    Name,
                    Description,
                    Shots {
                        <spellIds> {
                            Name,
                            Shot,
                            Option
                        }
                    }
                };
                
                export Menu {
                    CharacterSelectScreen {
                        Characters {
                            Character1 Character([
                                Fireball,
                                Iceball
                            ]),
                            Character2 Character([
                                MagicSpell,
                                Idk
                            ])
                        }
                    },
                    RankSelectScreen {
                        <Difficulties> {
                            Name,
                            Description
                        }
                    }
                };
                
                """
            );
            
            Console.WriteLine(result);
        }
    }
}
