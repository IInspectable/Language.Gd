using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class GlyphTests {

        [Test]
        public void EnsureMatchingControlTypeInGlyphEnumeration() {

            foreach (var controlType in Enum.GetValues(typeof(ControlType)).OfType<ControlType>()) {
                Assert.That(Enum.TryParse<Glyph>(controlType.ToString(), out _),
                            $"Es gibt in der {typeof(Glyph)} Enumeration keinen entsprechenden Member für den {typeof(ControlType)}.{controlType}");
            }

        }
        //[Test]
        //public void CheckGlyphMapping() {

        //    var sb = new StringBuilder();

        //    foreach (var controlType in Enum.GetValues(typeof(Glyph)).OfType<Glyph>()) {

        //        sb.AppendLine("{" + $"Glyph.{controlType}, KnownMonikers.{controlType}" + "},");
        //    }

        //    Assert.That(false, sb.ToString());
        //}

    }

}