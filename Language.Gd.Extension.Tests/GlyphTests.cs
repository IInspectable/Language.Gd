using System;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.Imaging.Interop;

using NUnit.Framework;

using Pharmatechnik.Language.Gd.Extension.Imaging;

namespace Pharmatechnik.Language.Gd.Extension.Tests {

    [TestFixture]
    public class GlyphTests {

        [Test]
        public void CheckGlyphMapping() {

            foreach (var glyph in Enum.GetValues(typeof(Glyph)).OfType<Glyph>()) {

                if (!GdImageMonikers.HasImageMoniker(glyph)) {

                    var msg = $"Es exisitert kein Mapping für Glyph.{glyph} in der Klasse {typeof(GdImageMonikers)}.Mapping\r\n" +
                              "Folgender Eintrag fehlt:\r\n" +
                              $"{{Glyph.{glyph}, KnownMonikers.{glyph}}}";

                    Assert.That(false, msg);
                }

            }

        }

        [Test]
        public void CheckFromGlyph() {

            foreach (var glyph in Enum.GetValues(typeof(Glyph)).OfType<Glyph>()) {
                var moniker = GdImageMonikers.GetMoniker(glyph);
                if (glyph != Glyph.None) {

                    Assert.That(moniker, Is.Not.EqualTo(default(ImageMoniker)));
                } else {
                    Assert.That(moniker, Is.EqualTo(default(ImageMoniker)));
                }

            }

        }

    }

}