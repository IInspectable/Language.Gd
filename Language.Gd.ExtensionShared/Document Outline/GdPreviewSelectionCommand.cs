#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class GdPreviewSelectionCommand: Command {

        private GdPreviewSelectionCommand(AsyncPackage package, OleMenuCommandService commandService)
            : base(package, commandService, PackageIds.GdPreviewSelectionId) {

            Instance = this;
        }

        [CanBeNull]
        public static GdPreviewSelectionCommand Instance { get; private set;}

        public override void Execute() {
            ThreadHelper.ThrowIfNotOnUIThread();
            GdLanguagePackage.InvokeCommand(IxosEssentialsCommandIds.GuiPreviewCommandId);
        }

        protected override bool ShouldEnable() {
            return GdOutlineToolWindowPane.Instance?.HasContent == true;
        }

        [UsedImplicitly]
        public static GdPreviewSelectionCommand Register(AsyncPackage package, OleMenuCommandService commandService) {
            return new GdPreviewSelectionCommand(package, commandService);
        }

    }

}