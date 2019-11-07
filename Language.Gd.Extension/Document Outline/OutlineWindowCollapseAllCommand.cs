#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class OutlineWindowCollapseAllCommand: Command {

        public OutlineWindowCollapseAllCommand(AsyncPackage package, OleMenuCommandService commandService)
            : base(package, commandService, PackageIds.OutlineWindowCollapseAllId) {
        }

        protected override bool ShouldEnable() {
            return GdOutlineToolWindowPane.Instance?.HasContent == true;
        }

        public override void Execute() {
            GdOutlineToolWindowPane.Instance?.CollapseAll();
        }

        [UsedImplicitly]
        public static OutlineWindowCollapseAllCommand Register(AsyncPackage package, OleMenuCommandService commandService) {
            return new OutlineWindowCollapseAllCommand(package, commandService);
        }

    }

}