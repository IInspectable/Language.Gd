using NUnit.Framework;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class ClassificationTests {

        private const string TestGd = @"
// Foo
NAMESPACE ns
    DIALOG DemoDialog
        PROPERTIES
            Title = ""DemoDialog"" 
        END PROPERTIES
        CONTROLS
            
        END CONTROLS
    END DIALOG DemoDialog
END NAMESPACE

    ";

        [Test]
        public void SimpleSyntaxTest() {

            var source = SourceText.From(TestGd);
            var tree   = SyntaxTree.Parse(source);

            Assert.That(tree.Diagnostics.Length, Is.EqualTo(0));

            var classification = SyntaxClassifier.Classify(tree.Root);

            Assert.That(classification.Length, Is.EqualTo(50));

        }

    }

}