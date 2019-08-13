using System.Collections.Immutable;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class DummyTests {

        private const string TestGd = @"

NAMESPACE ns
    DIALOG DemoDialog
        PROPERTIES
            Title = ""DemoDialog"" 
        END PROPERTIES
        CONTROLS
            PANEL DialogHeader Template = Header
              
                CONTROLS
                    CONTROL Textbox t1
                        PROPERTIES
                            Text = ""T1"" 
                            Length = 20 
                        END PROPERTIES
                    END CONTROL t1

                    CONTROL Textbox t2
                        PROPERTIES
                            Text = ""T2"" 
                            Length = 20 
                        END PROPERTIES
                    END CONTROL t2

                    CONTROL Textbox t3
                        PROPERTIES
                            Text = ""T3"" 
                            Length = 20 
                        END PROPERTIES
                    END CONTROL t3

                    CONTROL Textbox t4
                        PROPERTIES
                            Text = ""T1"" 
                            Length = 20 
                        END PROPERTIES
                    END CONTROL t4

                END CONTROLS
            END PANEL DialogHeader
            PANEL DialogContent Template = Content
             
                CONTROLS
                    CONTROL Textbox tc
                        PROPERTIES
                            Text = ""Tc"" 
                            Length = 20 
                        END PROPERTIES
                    END CONTROL tc

                END CONTROLS
            END PANEL DialogContent
            BARMANAGER FooterBars
               
                CONTROLS
                    CONTROL FunctionButton l1
                        PROPERTIES
                            Text = ""F1"" 
                            HotKeyText = ""F1"" 
                            Align = Left 
                        END PROPERTIES
                    END CONTROL l1

                    CONTROL FunctionButton l2
                        PROPERTIES
                            Text = ""F1"" 
                            HotKeyText = ""F1"" 
                            Align = Left 
                        END PROPERTIES
                    END CONTROL l2

                    CONTROL FunctionButton m1
                        PROPERTIES
                            Text = ""F1"" 
                            HotKeyText = ""F1"" 
                            Align = Right 
                        END PROPERTIES
                    END CONTROL m1

                    CONTROL FunctionButton m2
                        PROPERTIES
                            Text = ""F1"" 
                            HotKeyText = ""F1"" 
                            Align = Left 
                        END PROPERTIES
                    END CONTROL m2

                    CONTROL FunctionButton r1
                        PROPERTIES
                            Text = ""F1"" 
                            HotKeyText = ""F1"" 
                            Align = Right 
                        END PROPERTIES
                    END CONTROL r1

                    CONTROL FunctionButton r2
                        PROPERTIES
                            Text = ""F1f"" 
                            HotKeyText = @""F1\\Foo"" 
                            Align = Right 
                            Test.Foo[1].Bar[3].x.y=3
                        END PROPERTIES
                    END CONTROL r2

                END CONTROLS
            END BARMANAGER FooterBars
        END CONTROLS
    END DIALOG DemoDialog

END NAMESPACE

    ";

        [Test]
        public void SimpleSyntaxTest() {

            var source = SourceText.From(TestGd);
            var tree   = SyntaxTree.Parse(source);

            var tokens = tree.Tokens;
            EnsureContinousTokens(tokens);
            Assert.That(tree.Diagnostics.Length, Is.EqualTo(0));
        }

        [Test]
        public void AllGds() {

            var testDir = @"c:\ws\xtplus\main";
            if (!Directory.Exists(testDir)) {
                Assert.Warn($"Test directory '{testDir}' does not exist.");
                return;
            }

            var count = 0;
            foreach (var file in Directory.EnumerateFiles(
                testDir, "*.gd", SearchOption.AllDirectories)) {

                count++;

                var txt    = File.ReadAllText(file);
                var source = SourceText.From(txt);
                var tree   = SyntaxTree.Parse(source);
                var tokens = tree.Tokens;
                EnsureContinousTokens(tokens);
                if (tree.Diagnostics.Length > 0) {
                    Assert.Warn($"{file}\r\n{tree.Diagnostics.First().Location}");
                }

            }

            Assert.Warn($"{count} files processed.");
        }

        void EnsureContinousTokens(ImmutableDictionary<int, TokenSlot> tokens) {

            var lastEnd = 0;

            foreach (var token in tokens) {

                // TODO Enable Tests
                //foreach (var trivia in token.LeadingTrivias) {
                //    var start=trivia.K
                //    Assert.That(trivia.Start, Is.EqualTo(lastEnd));
                //    lastEnd = trivia.End;
                //}

                //Assert.That(token.Start, Is.EqualTo(lastEnd));
                //lastEnd = token.End;

                //foreach (var trivia in token.TrailingTrivias) {
                //    Assert.That(trivia.Start, Is.EqualTo(lastEnd));
                //    lastEnd = trivia.End;
                //}
            }

            Assert.That(tokens.Last().Value.Kind, Is.EqualTo(SyntaxKind.Eof));

        }

    }

}