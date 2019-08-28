#region Using Directives

using System;
using System.Drawing;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Pharmatechnik.Language.Gd.Extension.Images;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.NavigationBar {

    static class NavigationBarImages {

        public static class Index {
            public const int ProjectNode     = 0;
            public const int TaskDeclaration = 1;
            public const int TaskDefinition  = 2;
            public const int TriggerSymbol   = 3;
        }

        static IImageHandle _imageListHandle;

        public static IntPtr GetImageList(Color backgroundColor, IVsImageService2 imageService) {

            ThreadHelper.ThrowIfNotOnUIThread();

            EnsureImageListHandle(imageService);

            IntPtr hImageList = GdLanguagePackage.GetImageList(_imageListHandle.Moniker, backgroundColor);

            return hImageList;
        }

        static void EnsureImageListHandle(IVsImageService2 imageService) {
            
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_imageListHandle != null) {
                return;
            }
            
            // TODO Images hinzufügen!
            var imageList = new ImageMonikerImageList(
                KnownMonikers.CSProjectNode,
                KnownMonikers.Control);

            _imageListHandle = imageService.AddCustomImageList(imageList);
        }
    }
}