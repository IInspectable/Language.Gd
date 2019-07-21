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

            if (context.FRAGMENT() == null)
            {
                Tokens.Add(new LexerRule
                {
                    Name = context.TOKEN_REF().GetText(),
                    Index = ++_currentIndex
                });
            }
            
            return base.VisitLexerRuleSpec(context);
        }

    }

}