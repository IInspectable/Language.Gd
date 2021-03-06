﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class DummyTests {

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
        public void SimpleSyntaxTest() {

            var source = SourceText.From(TestGd);
            var tree   = SyntaxTree.Parse(source);

            Assert.That(tree.Diagnostics.Length, Is.EqualTo(0));

            var gds = (GuiDescriptionSyntax) tree.Root;

            EnsureContinousNodes(0, gds);
            EnsureContinousTokens(tree.Root);

            // NAMESPACE
            var namespaceToken = gds.FindToken(0);
            Assert.That(namespaceToken.Kind,      Is.EqualTo(SyntaxKind.Namespace));
            Assert.That(namespaceToken.IsKeyword, Is.True);

            // ns
            var nsToken = gds.FindToken(namespaceToken.EndPosition);
            Assert.That(nsToken.Kind, Is.EqualTo(SyntaxKind.Identifier));

            // DIALOG
            var dialogToken = gds.FindToken(nsToken.EndPosition);
            Assert.That(dialogToken.Kind,      Is.EqualTo(SyntaxKind.Dialog));
            Assert.That(dialogToken.IsKeyword, Is.True);

            var eofToken = gds.FindToken(source.Length);
            Assert.That(eofToken.Kind, Is.EqualTo(SyntaxKind.Eof));

            // Token Equalitiy
            var tl1 = gds.DescendantTokens().ToList();
            var tl2 = gds.DescendantTokens().ToList();

            for (int i = 0; i < tl1.Count; i++) {

                var t1 = tl1[i];
                var t2 = tl2[i];
                Assert.That(t1, Is.EqualTo(t2));
            }

            var firstToken = gds.FirstToken();
            var lastToken  = gds.LastToken();

            Assert.That(firstToken.Kind, Is.EqualTo(SyntaxKind.Namespace));
            Assert.That(lastToken.Kind,  Is.EqualTo(SyntaxKind.Eof));
        }

        [Test]
        public void ParseInvalidGd() {

            var text   = "? @Foo DIALOG";
            var source = SourceText.From(text);
            var tree   = SyntaxTree.Parse(source);

            Assert.That(tree.Root.Kind, Is.EqualTo(SyntaxKind.GuiDescriptionSyntax));
            var tokens = tree.Root.DescendantTokens().ToList();

            Assert.That(tokens.Count, Is.EqualTo(1)); // Nur Eof
            var eof = tokens.Last();

            // Der ganze "Mist" hängt als "Leading Trivia" am Eof Token
            Assert.That(eof.Kind,       Is.EqualTo(SyntaxKind.Eof));
            Assert.That(eof.Position,   Is.EqualTo(0));
            Assert.That(eof.FullLength, Is.EqualTo(text.Length));

            Assert.That(eof.LeadingTrivia.Count, Is.EqualTo(6));

            Assert.That(eof.LeadingTrivia[0].Kind, Is.EqualTo(SyntaxKind.Questionmark));
            Assert.That(eof.LeadingTrivia[0].Text, Is.EqualTo("?"));

            Assert.That(eof.LeadingTrivia[1].Kind, Is.EqualTo(SyntaxKind.WhitespaceTrivia));
            Assert.That(eof.LeadingTrivia[1].Text, Is.EqualTo(" "));

            Assert.That(eof.LeadingTrivia[2].Kind, Is.EqualTo(SyntaxKind.Unknown));
            Assert.That(eof.LeadingTrivia[2].Text, Is.EqualTo("@"));

            Assert.That(eof.LeadingTrivia[3].Kind, Is.EqualTo(SyntaxKind.Identifier));
            Assert.That(eof.LeadingTrivia[3].Text, Is.EqualTo("Foo"));

            Assert.That(eof.LeadingTrivia[4].Kind, Is.EqualTo(SyntaxKind.WhitespaceTrivia));
            Assert.That(eof.LeadingTrivia[4].Text, Is.EqualTo(" "));

            Assert.That(eof.LeadingTrivia[5].Kind, Is.EqualTo(SyntaxKind.Dialog));
            Assert.That(eof.LeadingTrivia[5].Text, Is.EqualTo("DIALOG"));

        }

        [Test]
        public void AllGds() {

            var testDir = @"c:\ws\xtplus\main";
            if (!Directory.Exists(testDir)) {
                Assert.Warn($"Test directory '{testDir}' does not exist.");
                return;
            }

            var stats = new SyntaxStatistics();
            var count = 0;
            foreach (var file in Directory.EnumerateFiles(
                testDir, "*.gd", SearchOption.AllDirectories)) {

                count++;

                var txt    = File.ReadAllText(file);
                var source = SourceText.From(txt);
                var tree   = SyntaxTree.Parse(source);

                EnsureContinousNodes(0, tree.Root);
                EnsureContinousTokens(tree.Root);

                //MeasureTrivias(tree.Root, stats);

                if (tree.Diagnostics.Length > 0) {
                    Assert.Warn($"{file}\r\n{tree.Diagnostics.First().Location}");
                }
            }

            TestContext.Out.WriteLine(stats.ToString());
            TestContext.Out.WriteLine($"{count} files processed.");
        }

        class SyntaxStatistics {

            public SyntaxStatistics() {
                LeadingTriviaStats  = new Dictionary<int, int>();
                TrailingTriviaStats = new Dictionary<int, int>();
            }

            public Dictionary<int, int> LeadingTriviaStats  { get; }
            public Dictionary<int, int> TrailingTriviaStats { get; }

            public void MeasureTrivia(SyntaxToken token) {

                var leadingTriviaCount = token.LeadingTrivia.Count;
                LeadingTriviaStats.TryGetValue(leadingTriviaCount, out var leadingTriviaCountUsed);
                LeadingTriviaStats[leadingTriviaCount] = leadingTriviaCountUsed + 1;

                var trailingTriviaCount = token.TrailingTrivia.Count;
                TrailingTriviaStats.TryGetValue(trailingTriviaCount, out var trailingTriviaCountUsed);
                TrailingTriviaStats[trailingTriviaCount] = trailingTriviaCountUsed + 1;
            }

            public override string ToString() {
                var sb = new StringBuilder();

                foreach (var entry in LeadingTriviaStats.OrderBy(kvp => kvp.Key)) {
                    sb.AppendLine($"LeadingTrivia: trivia count {entry.Key} => used {entry.Value}.");
                }

                foreach (var entry in TrailingTriviaStats.OrderBy(kvp => kvp.Key)) {
                    sb.AppendLine($"TrailingTrivia: trivia count {entry.Key} => used {entry.Value}.");
                }

                return sb.ToString();
            }

        }

        // ReSharper disable once UnusedMember.Local
        void MeasureTrivias(SyntaxNode root, SyntaxStatistics statistics) {
            foreach (var token in root.DescendantTokens()) {
                statistics.MeasureTrivia(token);
            }
        }

        void EnsureContinousNodes(int pos, SyntaxNode rootNode) {

            var lastEnd = pos;

            Assert.That(rootNode.Position, Is.EqualTo(pos));

            foreach (var nodeOrToken in rootNode.ChildNodesAndTokens()) {

                if (nodeOrToken.IsNode) {
                    EnsureContinousNodes(lastEnd, nodeOrToken.AsNode());
                }

                Assert.That(nodeOrToken.Position, Is.EqualTo(lastEnd));

                lastEnd = nodeOrToken.EndPosition;

            }

        }

        void EnsureContinousTokens(SyntaxNode rootNode) {
            var lastEnd = rootNode.Position;
            foreach (var token in rootNode.DescendantTokens()) {
                Assert.That(token.Position, Is.EqualTo(lastEnd));
                lastEnd = token.EndPosition;
            }
        }

    }

}