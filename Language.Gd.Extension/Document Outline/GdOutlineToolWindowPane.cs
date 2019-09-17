using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    [Guid("7e927358-0b4d-4953-b2bb-48ef216eb8cb")]
    public class GdOutlineToolWindowPane: ToolWindowPane {

        private readonly GdOutlineControl _outlineControl;

        public GdOutlineToolWindowPane(): base(null) {

            Caption            = "GD Outline";
            BitmapImageMoniker = KnownMonikers.DocumentOutline;
            _outlineControl    = new GdOutlineControl();
        }

        public override object Content {
            get => _outlineControl;
            set { }
        }

    }

}