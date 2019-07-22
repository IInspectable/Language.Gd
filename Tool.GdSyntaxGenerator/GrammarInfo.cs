using System;
using System.Collections.Generic;
using System.Linq;

using Antlr4.Runtime.Misc;

using Tool.GdSyntaxGenerator.Grammar;

namespace Tool.GdSyntaxGenerator {

    class GrammarInfo: AntlrV4GrammarBaseVisitor<string> {

        public GrammarInfo(AntlrV4Grammar.GrammarSpecContext tree) {
            tree.Accept(this);
            Verify();
        }

        public List<ParserRule> Rules { get; } = new List<ParserRule>();

        public void Verify() {
            foreach (var rule in Rules) {
                if (rule.Alternatives.Count > 1) {
                    //=> Wird zur abstrakten Basisklasse
                    var r = rule.Alternatives.Where(a => a.Elements.Count != 1);
                    if (r.Any()) {
                        throw new Exception($"Alternative darf nur aus einer Regelreferenz bestehen: {rule.Name}");
                    }

                    var tokens = rule.Alternatives.SelectMany(a => a.Elements.OfType<TokenElement>());
                    foreach (var token in tokens) {
                        // Sonst kann keine Klassenhierarchie erzeugt werden
                        throw new Exception($"Keine Token in Alternativen erlaubt: {rule.Name} -> {token.Name}");
                    }
                }
            }
        }

        ParserRule _currentRule;

        public override string VisitParserRuleSpec([NotNull] AntlrV4Grammar.ParserRuleSpecContext context) {

            _currentRule = new ParserRule {
                Name = context.RULE_REF().GetText()
            };

            Rules.Add(_currentRule);

            return base.VisitParserRuleSpec(context);
        }

        Alternative _currentAlternative;

        public override string VisitAlternative([NotNull] AntlrV4Grammar.AlternativeContext context) {

            _currentAlternative = new Alternative();
            _currentRule.Alternatives.Add(_currentAlternative);

            var r = base.VisitAlternative(context);
            _currentAlternative = null;
            return r;
        }

        public override string VisitElement([NotNull] AntlrV4Grammar.ElementContext context) {
            // labeledElelemnt atom ebnfSuffix => *

            var r = base.VisitElement(context);
            _currentElement = null;

            return r;
        }

        string _labeledElementName;

        public override string VisitLabeledElement([NotNull] AntlrV4Grammar.LabeledElementContext context) {
            _labeledElementName = context.identifier().GetText();
            var result = base.VisitLabeledElement(context);
            _labeledElementName = null;
            return result;
        }

        private Element _currentElement;

        public override string VisitRuleref([NotNull] AntlrV4Grammar.RulerefContext context) {
            // context.RULE_REF() => name der Regel

            if (_currentAlternative != null) {
                _currentElement = new RuleElement {
                    SyntaxKind = $"{context.RULE_REF().GetText()}Syntax",
                    Name       = _labeledElementName ?? context.RULE_REF().GetText(),
                    IsLabeled  = _labeledElementName != null
                };

                _currentAlternative.Elements.Add(_currentElement);
            }

            return base.VisitRuleref(context);
        }

        public override string VisitTerminal([NotNull] AntlrV4Grammar.TerminalContext context) {

            if (_currentAlternative != null) {
                _currentElement = new TokenElement {
                    Name       = _labeledElementName ?? context.TOKEN_REF().GetText(),
                    SyntaxKind = $"{context.TOKEN_REF().GetText()}",
                    IsLabeled  = _labeledElementName != null
                };

                _currentAlternative.Elements.Add(_currentElement);
            }

            var r = base.VisitTerminal(context);

            return r;
        }

        public override string VisitEbnfSuffix([NotNull] AntlrV4Grammar.EbnfSuffixContext context) {

            if (_currentElement != null) {
                var suffix = context.GetText();
                _currentElement.Cardinality = suffix;
            } else {
                throw new Exception($"kann Kardinalität nicht setzen: {context.GetText()}");
            }

            return base.VisitEbnfSuffix(context);
        }

        //bool inBlock = false;

        public override string VisitBlock([NotNull] AntlrV4Grammar.BlockContext context) {

            throw new Exception($"Blocks are not supported: {context.GetText()}");

            //  return base.VisitBlock(context);
        }

    }

}