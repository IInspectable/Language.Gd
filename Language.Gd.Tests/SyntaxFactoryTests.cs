using NUnit.Framework;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class SyntaxFactoryTests {

        [Test]
        public void TestParsePropertiesSection() {
            string props = @"PROPERTIES
                END PROPERTIES";

            var node = SyntaxFactory.ParsePropertiesSectionSyntax(props);

            Assert.That(node,                               Is.Not.Null);
            Assert.That(node.GetFullText(),                 Is.EqualTo(props));
            Assert.That(node.SyntaxTree.Diagnostics.Length, Is.EqualTo(0));

        }

        [Test]
        public void TestParseWrongPropertiesSection() {
            string props = @"{
                END PROPERTIES";

            var node = SyntaxFactory.ParsePropertiesSectionSyntax(props);

            Assert.That(node,                               Is.Not.Null);
            Assert.That(node.GetFullText(),                 Is.EqualTo(props));
            Assert.That(node.SyntaxTree.Diagnostics.Length, Is.EqualTo(1));

            Assert.That(node.PropertiesSectionBegin?.IsMissing, Is.True);
            Assert.That(node.Propertys.Count,                   Is.EqualTo(0));
            Assert.That(node.PropertiesSectionEnd?.IsMissing,   Is.False);

        }

    }

}