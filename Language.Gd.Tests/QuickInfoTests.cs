using NUnit.Framework;

using Pharmatechnik.Language.Gd.QuickInfo;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class QuickInfoTests {

        [Test]
        public void NamespaceQuickInfo() {

            string testGd = @"NAMESPACE    Pharmatechnik.Apotheke.XTplus.Verkauf.GUI   ";

            var syntax = SyntaxFactory.ParseNamespaceSectionBeginSyntax(testGd);

            Assert.That(syntax.SyntaxTree, Is.Not.Null);
            Assert.That(syntax.SyntaxTree.Diagnostics.Length, Is.EqualTo(0));

            var qiDef=QuickInfoProvider.GetQuickInfoDefinition(syntax.SyntaxTree, 0);
            Assert.That(qiDef, Is.Null);
            qiDef = QuickInfoProvider.GetQuickInfoDefinition(syntax.SyntaxTree, 12);
            Assert.That(qiDef, Is.Null);

            qiDef = QuickInfoProvider.GetQuickInfoDefinition(syntax.SyntaxTree, 13);
            Assert.That(qiDef, Is.Not.Null);
            Assert.That(qiDef.Glyph, Is.EqualTo(Glyph.Namespace));
            Assert.That(qiDef.Content.JoinText(), Is.EqualTo("NAMESPACE Pharmatechnik.Apotheke.XTplus.Verkauf.GUI"));

            qiDef = QuickInfoProvider.GetQuickInfoDefinition(syntax.SyntaxTree, 27);
            Assert.That(qiDef, Is.Not.Null);

            qiDef = QuickInfoProvider.GetQuickInfoDefinition(syntax.SyntaxTree, 54);
            Assert.That(qiDef, Is.Not.Null);

            qiDef = QuickInfoProvider.GetQuickInfoDefinition(syntax.SyntaxTree, 55);
            Assert.That(qiDef, Is.Null);
        }

    }

}