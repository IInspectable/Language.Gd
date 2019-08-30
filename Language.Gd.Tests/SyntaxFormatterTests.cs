using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class SyntaxFormatterTests {

        [Test]
        public void Bla() {

            string inputSyntax =
                @"
        PROPERTIES
a=b

        c=d
    END PROPERTIES
";
            string expectedSyntax =
                @"PROPERTIES
    a = b
    c = d
END PROPERTIES
";

            var gds = SyntaxFactory.ParsePropertiesSectionSyntax(inputSyntax);

            var writer = new TextFormatter();
            writer.Visit(gds);
            var actual = writer.ToString();

            Assert.That(actual, Is.EqualTo(expectedSyntax));
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

                //var lines = new List<string>();

                //using (var enumerator = node.DescendantTokens().GetEnumerator()) {

                //    if (enumerator.MoveNext()) {
                //        var token = enumerator.Current;
                       
                //        // Trivia erstes Token
                //        foreach (var trivia in token.LeadingTrivia) {
                //            if (trivia.Kind == SyntaxKind.SingleLineCommentTrivia) {
                //                lines.Add(trivia.Text);
                //            }
                //        }


                //    }

                //}

                var line = new StringBuilder();

                var separator = "";
                foreach (var token in node.DescendantTokens()) {
                    line.Append(separator + token.Text);
                    separator = " ";
                }

                var lineText = line.ToString();
                if (!string.IsNullOrWhiteSpace(lineText)) {
                    _sb.AppendLine(GetIndentString() + lineText);
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

}