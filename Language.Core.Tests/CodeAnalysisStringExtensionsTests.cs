using NUnit.Framework;

using Pharmatechnik.Language.CodeAnalysis;

namespace Pharmatechnik.Language.Core.Tests {

    [TestFixture]
    public class CodeAnalysisStringExtensionsTests {

       // [Test]
        public void TestInQuot1() {

            var comment =
                @"/*

  Foo
*/
";

            var expected =
                @"        /*

          Foo
        */
";
            var actual = comment.ReindentMultilineComment(8, 4, false);

            Assert.That(actual, Is.EqualTo(expected));

            actual=expected.ReindentMultilineComment(0, 4, false);

            Assert.That(actual, Is.EqualTo(comment));
        }

    }

}