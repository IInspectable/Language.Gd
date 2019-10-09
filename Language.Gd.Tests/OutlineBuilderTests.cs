using NUnit.Framework;

using Pharmatechnik.Language.Gd.DocumentOutline;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class OutlineBuilderTests {

        private const string TestGd = @"
// Foo
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
        public void Test() {

            var source = SourceText.From(TestGd);
            var tree   = SyntaxTree.Parse(source);

            Assert.That(tree.Diagnostics.Length, Is.EqualTo(0));

            var outlineElement = OutlineBuilder.Build(tree.Root as GuiDescriptionSyntax, detailed: true);

            Assert.That(outlineElement,                 Is.Not.Null);
            Assert.That(outlineElement.Children.Length, Is.EqualTo(3));
        }

    }

}