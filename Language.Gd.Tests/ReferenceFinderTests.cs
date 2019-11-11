using NUnit.Framework;
// ReSharper disable UnusedVariable

namespace Pharmatechnik.Language.Gd.Tests {

    [TestFixture]
    public class ReferenceFinderTests {

        [Test]
        public void A() {

            var r = new Reference(
                "C:\\ws\\xtplus\\z_eKv2020.2\\XTplusApplication\\src\\XTplus.Faktura\\FakturaUebersicht\\AuftragVorlaeufer\\EkvInfo.gd",
                "Pharmatechnik.Apotheke.XTplus.Faktura.FakturaUebersicht.AuftragVorlaeufer.GUI.EkvInfoRückmeldungen");
            ReferenceFinder.TryResolveFileName(r, out var filename);

           

            var p=r.GetSuggestedPaths();
        }

    }

}