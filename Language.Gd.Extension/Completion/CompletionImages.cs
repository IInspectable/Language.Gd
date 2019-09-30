#region Using Directives

using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Text.Adornments;

using Pharmatechnik.Language.Gd.Extension.Imaging;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Completion {

    static class CompletionImages {

        public static ImageElement Keyword = new ImageElement(ImageMonikers.Keyword.ToImageId());

    }

}