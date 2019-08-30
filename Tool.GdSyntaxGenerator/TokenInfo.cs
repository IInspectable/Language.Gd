using System;
using System.Collections.Generic;

using Antlr4.Runtime.Misc;

using Tool.GdSyntaxGenerator.Grammar;

namespace Tool.GdSyntaxGenerator {

    class TokenInfo: AntlrV4GrammarBaseVisitor<string> {

        public List<LexerRule> Tokens { get; } = new List<LexerRule>();

        public TokenInfo(AntlrV4Grammar.GrammarSpecContext tree) {

            Tokens.Add(new LexerRule {
                Name  = nameof(AntlrV4Grammar.Eof),
                Index = AntlrV4Grammar.Eof
            });

            _currentIndex = 0;
            tree.Accept(this);
        }

        private int _currentIndex;

        public override string VisitLexerRuleSpec([NotNull] AntlrV4Grammar.LexerRuleSpecContext context) {

            if (context.FRAGMENT() == null) {
                Tokens.Add(new LexerRule {
                    Name             = context.TOKEN_REF().GetText(),
                    Index            = ++_currentIndex,
                    IsSimpleTerminal = IsSimpleTextToken(context, out var text),
                    TerminalText     = text
                });
            }

            return base.VisitLexerRuleSpec(context);
        }

        bool IsSimpleTextToken([NotNull] AntlrV4Grammar.LexerRuleSpecContext context, out string text) {

            text = context.lexerRuleBlock()?.lexerAltList()
                          .lexerAlt()?.SingleIfOnly()
                         ?.lexerElements()
                         ?.lexerElement()?.SingleIfOnly()
                         ?.lexerAtom()
                         ?.terminal()
                         ?.STRING_LITERAL()
                         ?.GetText();

            if (text != null) {
                if (text.StartsWith("'")) {
                    text = text.Substring(1, text.Length - 1);
                }
                if (text.EndsWith("'")) {
                    text = text.Substring(0, text.Length - 1);
                }
            }

            return !String.IsNullOrEmpty(text);
        }

    }

    static class Extensions {

        public static T SingleIfOnly<T>(this IEnumerable<T> source) where T : class {

            using (var iter = source.GetEnumerator()) {

                // Kein Element
                if (!iter.MoveNext()) {
                    return default;
                }

                var elem = iter.Current;

                // Mehr als ein Element
                if (iter.MoveNext()) {
                    return null;
                }

                return elem;

            }

        }

    }

}