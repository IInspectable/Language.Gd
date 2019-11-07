#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class GdGenerateSelectionCommand: Command {

        private GdGenerateSelectionCommand(AsyncPackage package, OleMenuCommandService commandService)
            : base(package, commandService, PackageIds.GdGenerateSelectionId) {

            Instance = this;
        }

        [CanBeNull]
        public static GdGenerateSelectionCommand Instance { get; private set;}

        public override void Execute() {
            ThreadHelper.ThrowIfNotOnUIThread();
            GdLanguagePackage.InvokeCommand(IxosEssentialsCommandIds.GdGenerateCommandId);
        }

        protected override bool ShouldEnable() {
            return GdOutlineToolWindowPane.Instance?.HasContent == true;
        }

        [UsedImplicitly]
        public static GdGenerateSelectionCommand Register(AsyncPackage package, OleMenuCommandService commandService) {
            return new GdGenerateSelectionCommand(package, commandService);
        }

    }

}