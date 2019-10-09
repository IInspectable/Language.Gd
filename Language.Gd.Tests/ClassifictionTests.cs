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

            var parts = tree.Root.ToSimplifiedText();

            var txt = parts.JoinText();
            var expected = 
@"NAMESPACE ns
    DIALOG DemoDialog
        PROPERTIES
            Title = ""DemoDialog""
        END PROPERTIES
        CONTROLS
        END CONTROLS
    END DIALOG DemoDialog
END NAMESPACE
";
            Assert.That(txt, Is.EqualTo(expected));

            var editorSettings = TextEditorSettings.Default.With(tabSize: 2);
            parts = tree.Root.ToSimplifiedText(editorSettings);
            txt   = parts.JoinText();
            expected = 
@"NAMESPACE ns
  DIALOG DemoDialog
    PROPERTIES
      Title = ""DemoDialog""
    END PROPERTIES
    CONTROLS
    END CONTROLS
  END DIALOG DemoDialog
END NAMESPACE
";
            Assert.That(txt, Is.EqualTo(expected));
        }

        [Test]
        public void ControlSectionBeginSyntax() {

            var source = SourceText.From("CONTROL Textbox txtTest");
            var syntax = SyntaxFactory.ParseControlSectionBeginSyntax(source);
            var tree   = syntax.SyntaxTree;

            Assert.That(tree, Is.Not.Null);
            Assert.That(tree.Diagnostics.Length, Is.EqualTo(0));

            var parts = syntax.ToSimplifiedText();

            Assert.That(parts.Length, Is.EqualTo(5));

        }

        [Test]
        public void PropertiesSectionSyntax() {

            var source = SourceText.From("PROPERTIES Text=\"Bla\" Align[5]=\"Left\" Cell[0].Changed+=OnChanged END PROPERTIES ");
            var syntax = SyntaxFactory.ParsePropertiesSectionSyntax(source);
            var tree   = syntax.SyntaxTree;

            Assert.That(tree, Is.Not.Null);
            Assert.That(tree.Diagnostics.Length, Is.EqualTo(0));

            var parts = syntax.ToSimplifiedText();

            Assert.That(parts.Length, Is.EqualTo(34));

            var txt = parts.JoinText();

            var expected =
                @"PROPERTIES
    Text = ""Bla""
    Align[5] = ""Left""
    Cell[0].Changed += OnChanged
END PROPERTIES";
            Assert.That(txt, Is.EqualTo(expected));

        }

        [Test]
        public void QualifiedNameSyntax() {

            var source = SourceText.From(" Foo.Bar\r\n . Baz ");
            var syntax = SyntaxFactory.ParseQualifiedNameSyntax(source);
            var tree   = syntax.SyntaxTree;

            Assert.That(tree, Is.Not.Null);
            Assert.That(tree.Diagnostics.Length, Is.EqualTo(0));

            var parts = syntax.ToSimplifiedText();

            Assert.That(parts.Length, Is.EqualTo(5));

            var txt = parts.JoinText();
            Assert.That(txt, Is.EqualTo("Foo.Bar.Baz"));

        }

        [Test]
        public void HotkeyDeclarationSyntax() {

            var source = SourceText.From(" HOTKEY\r\nF9, \r\n+CTRL,+ALT    +=CtrlF9_RezeptschuldAuflösen CustomCall");
            var syntax = SyntaxFactory.ParseHotkeyDeclarationSyntax(source);
            var tree   = syntax.SyntaxTree;

            Assert.That(tree, Is.Not.Null);
            Assert.That(tree.Diagnostics.Length, Is.EqualTo(0));

            var parts = syntax.ToSimplifiedText();

            Assert.That(parts.Length, Is.EqualTo(15));

            var txt = parts.JoinText();
            Assert.That(txt, Is.EqualTo("HOTKEY F9, +CTRL, +ALT += CtrlF9_RezeptschuldAuflösen CustomCall"));

        }

    }

}