#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    partial class SimplifiedClassificationBuilder: SyntaxListener {

        readonly List<ClassifiedText> _parts;
        int                           _indentLevel;
        bool                          _emptyLine;

        SimplifiedClassificationBuilder(TextEditorSettings editorSettings): base(SyntaxListenerDepth.Token) {

            editorSettings = editorSettings ?? TextEditorSettings.Default;
            _parts         = new List<ClassifiedText>();
            _emptyLine     = true;
            _indentLevel   = 0;

            WhiteSpace = new ClassifiedText(" ",                                     GdClassification.WhiteSpace);
            NewLine    = new ClassifiedText(editorSettings.NewLine,                  GdClassification.WhiteSpace);
            Tab        = new ClassifiedText(new String(' ', editorSettings.TabSize), GdClassification.WhiteSpace);
        }

        ClassifiedText WhiteSpace { get; }
        ClassifiedText Tab        { get; }
        ClassifiedText NewLine    { get; }

        public static ImmutableArray<ClassifiedText> Classify(SyntaxToken token, TextEditorSettings editorSettings) {

            var builder = new SimplifiedClassificationBuilder(editorSettings);

            builder.Visit(token);
            builder.TrimEnd();

            return builder._parts.ToImmutableArray();
        }

        public static ImmutableArray<ClassifiedText> Classify(SyntaxNode node, TextEditorSettings editorSettings) {

            var builder = new SimplifiedClassificationBuilder(editorSettings);

            builder.Visit(node);
            builder.TrimEnd();

            return builder._parts.ToImmutableArray();
        }

        protected override void DefaultVisit(SyntaxNode node) {

            if (node.Parent is ISectionSyntax s1 && node == s1.SectionEnd) {
                DecreaseIndent();
                AddNewLine();
            }

            base.DefaultVisit(node);

            // Unmittelbare Kinder einer Sektion landen immer auf einer eigenen Zeile
            if (node.Parent is ISectionSyntax) {
                AddNewLine();
            }

            if (node.Parent is ISectionSyntax s2 && node == s2.SectionBegin) {
                IncreaseIndent();
                AddNewLine();
            }
        }

        protected override void VisitToken(SyntaxToken token) {

            if (token.IsMissing) {
                return;
            }

            AddTokenSeparator();
            Add(token.GetText(), SyntaxClassifierHelper.ClassifyToken(token));

            // Komma Verschiebung: Eigentlich gehört das Komma zur ModifierSyntax,
            // wir wollen das Leerzeichen aber rechts vom Komma haben:
            // => F9, +CTRL, +ALT
            //      ^_     ^_
            //
            if (token.Kind == SyntaxKind.Comma && token.Parent is ModifierOptionSyntax) {
                AddWhitespace();
            }
        }

        protected internal override void VisitQualifiedNameSyntax(QualifiedNameSyntax qualifiedName) {
            // Nur den ersten Separator, die "Continuations" sollen kein Leerzeichen nach dem Punkt haben.
            // _> NAMESPACE Foo.Bar.Bar
            //                 ^- Vor und nach diesem Punkt kein Leerzeichen!
            //             ^- Dieses Leerzeichen wollen wir noch
            AddTokenSeparator();
            using (SupressTokenSeparator()) {
                base.VisitQualifiedNameSyntax(qualifiedName);
            }
        }

        protected internal override void VisitLvalueExpressionSyntax(LvalueExpressionSyntax lvalueExpression) {
            using (SupressTokenSeparator()) {
                base.VisitLvalueExpressionSyntax(lvalueExpression);
            }
        }

        protected internal override void VisitModifierOptionSyntax(ModifierOptionSyntax modifierOption) {
            using (SupressTokenSeparator()) {
                base.VisitModifierOptionSyntax(modifierOption);
            }
        }

        private void IncreaseIndent() {
            _indentLevel++;
        }

        private void DecreaseIndent() {
            _indentLevel--;
        }

        void Add(string text, GdClassification classification) {
            EnsureIndent();
            _parts.Add(new ClassifiedText(text, classification));
        }

        void AddWhitespace() {

            if (_emptyLine) {
                // Macht keinen Sinn
                return;
            }

            _parts.Add(WhiteSpace);
        }

        void EnsureIndent() {

            if (!_emptyLine) {
                return;
            }

            for (int i = 0; i < _indentLevel; ++i) {
                _parts.Add(Tab);
            }

            _emptyLine = false;
        }

        void AddNewLine() {

            if (_emptyLine) {
                return;
            }

            TrimEndWhiteSpace();

            _parts.Add(NewLine);
            _emptyLine = true;
        }

        void TrimEndWhiteSpace() {

            int idx = _parts.Count;
            while (idx > 0) {
                idx--;
                if (_parts[idx] != WhiteSpace) {
                    idx++;
                    break;
                }
            }

            if (idx < _parts.Count) {
                _parts.RemoveRange(idx, _parts.Count - idx);
            }
        }

        void TrimEnd() {

            int idx = _parts.Count;
            while (idx > 0) {
                idx--;
                if (_parts[idx] != WhiteSpace &&
                    _parts[idx] != NewLine) {
                    idx++;
                    break;
                }
            }

            if (idx < _parts.Count) {
                _parts.RemoveRange(idx, _parts.Count - idx);
            }
        }

    }

}