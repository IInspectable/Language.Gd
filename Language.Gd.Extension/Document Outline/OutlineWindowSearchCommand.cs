#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    internal sealed class OutlineWindowSearchCommand: Command {

        private OutlineWindowSearchCommand(AsyncPackage package, OleMenuCommandService commandService):
            base(package, commandService,
                 PackageIds.OutlineWindowSearchId) {

            Instance = this;
        }

        [UsedImplicitly]
        public static OutlineWindowSearchCommand Instance { get; private set; }

        [UsedImplicitly]
        public static OutlineWindowSearchCommand Register(AsyncPackage package, OleMenuCommandService commandService) {
            return new OutlineWindowSearchCommand(package, commandService);
        }

        protected override bool ShouldEnable() => true;

        public override void Execute() {
            ThreadHelper.ThrowIfNotOnUIThread();
            OutlineWindowShowCommand.Instance?.Execute();
            GdOutlineToolWindowPane.Instance?.ActivateSearch();
        }

    }

}