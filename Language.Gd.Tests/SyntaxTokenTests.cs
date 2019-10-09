#region Using Directives

using NUnit.Framework;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class SyntaxTokenTests {

        [Test]
        public void TestDefaultToken() {

            SyntaxToken token = default;

            Assert.That(token, Is.EqualTo(default(SyntaxToken)));

            Assert.That(token.SyntaxTree, Is.Null);
            Assert.That(token.GetText(),  Is.EqualTo(""));

            Assert.That(token.Slot,   Is.Null);
            Assert.That(token.Parent, Is.Null);

            Assert.That(token.IsMissing, Is.False);

            Assert.That(token.Kind,      Is.EqualTo(SyntaxKind.None));
            Assert.That(token.IsKeyword, Is.False);

            Assert.That(token.Position,    Is.EqualTo(0));
            Assert.That(token.EndPosition, Is.EqualTo(0));

            Assert.That(token.Length,     Is.EqualTo(0));
            Assert.That(token.FullLength, Is.EqualTo(0));

            Assert.That(token.ExtentStart, Is.EqualTo(0));
            Assert.That(token.Extent,      Is.EqualTo(default(TextExtent)));
            Assert.That(token.FullExtent,  Is.EqualTo(default(TextExtent)));

            Assert.That(token.HasLeadingTrivia, Is.False);
            Assert.That(token.LeadingWidth,     Is.EqualTo(0));
            Assert.That(token.LeadingTrivia,    Is.Empty);

            Assert.That(token.HasTrailingTrivia, Is.False);
            Assert.That(token.TrailingWidth,     Is.EqualTo(0));
            Assert.That(token.TrailingTrivia,    Is.Empty);

        }

    }

}