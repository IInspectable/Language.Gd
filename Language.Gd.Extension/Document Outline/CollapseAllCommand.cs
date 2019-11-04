#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class CollapseAllCommand: Command {

        public CollapseAllCommand(AsyncPackage package, OleMenuCommandService commandService): base(package, commandService, PackageIds.CollapseAllId) {
        }

        protected override bool ShouldEnable() {
            return GdOutlineToolWindowPane.Instance?.CanExpandCollapse == true;
        }

        public override void Execute() {
            GdOutlineToolWindowPane.Instance?.CollapseAll();
        }

        [UsedImplicitly]
        public static CollapseAllCommand Register(AsyncPackage package, OleMenuCommandService commandService) {
            return new CollapseAllCommand(package, commandService);
        }

    }

}