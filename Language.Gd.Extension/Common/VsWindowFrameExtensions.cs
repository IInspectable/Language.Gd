#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class VsWindowFrameExtensions {

        [CanBeNull]
        public static IWpfTextView ToWpfTextView(this IVsWindowFrame vsWindowFrame) {

            var vsTextView = VsShellUtilities.GetTextView(vsWindowFrame);

            if (vsTextView != null) {
                var guidTextViewHost = DefGuidList.guidIWpfTextViewHost;
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (((IVsUserData) vsTextView).GetData(ref guidTextViewHost, out var textViewHost) == VSConstants.S_OK)
                    return (textViewHost as IWpfTextViewHost)?.TextView;
            }

            return null;
        }

    }

}