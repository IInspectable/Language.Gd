using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class SyntaxFormatterTests {

        [Test]
        public void Bla() {

            string inputSyntax =
@"    // Comment 1
  // Comment 2
PROPERTIES
a=b 
   
        c=d   
    END PROPERTIES
";
            string expectedSyntax =
@"// Comment 1
// Comment 2
PROPERTIES
    a = b
    c = d
END PROPERTIES
";

            var gds = SyntaxFactory.ParsePropertiesSectionSyntax(inputSyntax);

            var writer = new TextFormatter();
            writer.Visit(gds);
            var actual = writer.ToString();

            Assert.That(actual, Is.EqualTo(expectedSyntax));

            var comment=
@"/*
 1
    2
     3
      */";
            SourceText st = SourceText.From(comment);

            var indent=st.TextLines.Select(tl => tl.GetIndentationColumn(tabSize:4)).ToList();

            Assert.That(indent, Is.EquivalentTo(new[] {0, 1, 4, 5, 6}));

        }

        class TextFormatter: SyntaxListener {

            readonly StringBuilder _sb;

            public TextFormatter(): base(SyntaxListenerDepth.Trivia) {
                _sb = new StringBuilder();
            }

            public override string ToString() {
                return _sb.ToString();
            }

            protected override void VisitToken(SyntaxToken token) {
                Write(token.Text);
            }

            protected override void VisitTrivia(SyntaxTrivia trivia) {
                Write(trivia.Text);
            }

            protected internal override void VisitPropertiesSectionBeginSyntax(PropertiesSectionBeginSyntax propertiesSectionBegin) {
                WriteSectionBegin(propertiesSectionBegin);
            }

            protected internal override void VisitPropertyAssignSyntax(PropertyAssignSyntax propertyAssign) {
                WriteOnSingleLine(propertyAssign);
            }

            protected internal override void VisitPropertiesSectionEndSyntax(PropertiesSectionEndSyntax propertiesSectionEnd) {
                WriteSectionEnd(propertiesSectionEnd);
            }

            void WriteSectionBegin(SyntaxNode sectionBegin) {
                WriteOnSingleLine(sectionBegin);
                Indent();
            }

            void WriteSectionEnd(SyntaxNode sectionEnd) {
                Unindent();
                WriteOnSingleLine(sectionEnd);
            }

            void Write(string text) {
                _sb.Append(text);
            }

            void WriteOnSingleLine(SyntaxNode node) {

                var builder = new LinesBuilder();

                using (var enumerator = node.DescendantTokens().GetEnumerator()) {

                    if (enumerator.MoveNext()) {

                        var token = enumerator.Current;

                        // Trivia vom ersten Token
                        foreach (var trivia in token.LeadingTrivia) {

                            if (trivia.Kind == SyntaxKind.SingleLineCommentTrivia) {
                                builder.WriteOnNewLine(trivia.Text);
                                builder.NewLine();
                            }

                            if (trivia.Kind == SyntaxKind.MultiLineCommentTrivia) {
                                builder.WriteOnNewLine(trivia.Text);
                                builder.NewLine();
                            }

                            if (trivia.IsSkipedTokenTrivia) {
                                builder.WritePart(token.Text);
                            }

                            // Whitespaces sind uninteressant
                        }

                        builder.WriteOnNewLine(token.Text);

                    }

                    while (enumerator.MoveNext()) {
                        var token = enumerator.Current;
                        builder.WritePart(token.Text);
                    }

                }

                //foreach (var token in node.DescendantTokens()) {
                //    builder.WritePart(token.Text);
                //}

                foreach (var line in builder.GetLines()) {
                    _sb.AppendLine(GetIndentString() + line);
                }

            }

            string GetIndentString() {
                return new string(' ', _indent * IndentSize);
            }

            private const int IndentSize = 4;

            void Indent() {
                _indent += 1;
            }

            void Unindent() {
                _indent -= 1;
            }

            private int _indent;

        }

    }

    class LinesBuilder {

        readonly LineBuilder _lineBuilder;

        readonly List<string> _lines;

        public LinesBuilder() {
            _lineBuilder = new LineBuilder();
            _lines       = new List<string>();
        }

        public List<string> GetLines() {
            NewLine();
            return _lines;
        }

        public void WritePart(string text) {
            _lineBuilder.WritePart(text);
        }

        public void NewLine() {
            if (_lineBuilder.TryCompleteLine(out var completedLine)) {
                _lines.Add(completedLine);
            }
        }

        public void WriteOnNewLine(string text) {
            NewLine();

            _lineBuilder.WritePart(text);

        }

    }

    class LineBuilder {

        private readonly List<string> _parts;

        public LineBuilder() {
            _parts = new List<string>();
        }

        public void WritePart(string text) {
            _parts.Add(text.Replace("\r\n",""));
        }

        public bool TryCompleteLine(out string completedLine) {

            completedLine = null;

            if (_parts.Any()) {

                completedLine = String.Join(" ", _parts);
                _parts.Clear();

                return true;

            }

            return false;
        }

    }

}