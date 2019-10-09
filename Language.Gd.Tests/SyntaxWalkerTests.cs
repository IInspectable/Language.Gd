using System.Text;

using NUnit.Framework;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class SyntaxWalkerTests {

        [Test]
        public void TestAllNodesCalled() {

            string testGd = @"
NAMESPACE Ns.a
    // Single Line Comment
    USER CONTROL User   
        PROPERTIES
        END PROPERTIES
        CONTROLS
        /*  Multiline
            Comment   */
        END CONTROLS
        @ // <- Unknown Token
    END USER CONTROL User
END NAMESPACE";

            var gds = SyntaxFactory.ParseGuiDescriptionSyntax(testGd);

            var writer = new TextWriter();
            writer.Visit(gds);

            Assert.That(writer.ToString(), Is.EqualTo(testGd));
        }

        class TextWriter: SyntaxListener {

            readonly StringBuilder _sb;

            public TextWriter(): base(SyntaxListenerDepth.Trivia) {
                _sb = new StringBuilder();
            }

            public override string ToString() {
                return _sb.ToString();
            }

            protected override void VisitToken(SyntaxToken token) {
                _sb.Append(token.GetText());
            }

            protected override void VisitTrivia(SyntaxTrivia trivia) {
                _sb.Append(trivia.Text);
            }

            protected internal override void VisitUserControlSectionBeginSyntax(UserControlSectionBeginSyntax userControlSectionBegin) {
                // Wir können auch einzelne Syntaxen "rausnehmen".
                _sb.Append(userControlSectionBegin.GetFullText());
            }

        }

    }

}