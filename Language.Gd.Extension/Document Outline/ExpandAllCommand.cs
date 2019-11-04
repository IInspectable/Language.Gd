#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class ExpandAllCommand: Command {

        public ExpandAllCommand(AsyncPackage package, OleMenuCommandService commandService): base(package, commandService, PackageIds.ExpandAllId) {
        }

        protected override bool ShouldEnable() {
            return GdOutlineToolWindowPane.Instance?.CanExpandCollapse == true;
        }

        public override void Execute() {
            GdOutlineToolWindowPane.Instance?.ExpandAll();
        }

        [UsedImplicitly]
        public static ExpandAllCommand Register(AsyncPackage package, OleMenuCommandService commandService) {
            return new ExpandAllCommand(package, commandService);
        }

    }

}