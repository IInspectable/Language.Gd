#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class OutlineWindowExpandAllCommand: Command {

        public OutlineWindowExpandAllCommand(AsyncPackage package, OleMenuCommandService commandService): base(package, commandService, PackageIds.OutlineWindowExpandAllId) {
        }

        protected override bool ShouldEnable() {
            return GdOutlineToolWindowPane.Instance?.HasContent == true;
        }

        public override void Execute() {
            GdOutlineToolWindowPane.Instance?.ExpandAll();
        }

        [UsedImplicitly]
        public static OutlineWindowExpandAllCommand Register(AsyncPackage package, OleMenuCommandService commandService) {
            return new OutlineWindowExpandAllCommand(package, commandService);
        }

    }

}