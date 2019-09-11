using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Pharmatechnik.Language.Gd.Extension.Imaging {

    static class ImageMonikers {

        public static ImageMoniker WaitingForAnalysis => KnownMonikers.Loading;
        public static ImageMoniker AnalysisOK         => KnownMonikers.StatusOK;
        public static ImageMoniker AnalysisWarning    => KnownMonikers.StatusWarning;
        public static ImageMoniker AnalysisError      => KnownMonikers.StatusError;

        public static ImageMoniker StatusInformation => KnownMonikers.StatusInformation;

    }

}