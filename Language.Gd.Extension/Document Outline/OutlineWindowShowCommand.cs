#region Using Directives

using System;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    internal sealed class OutlineWindowShowCommand: Command {

        public static OutlineWindowShowCommand Instance { get; private set; }

        private OutlineWindowShowCommand(AsyncPackage package, OleMenuCommandService commandService):
            base(package, commandService,
                 PackageIds.OutlineWindowShowId) {

            Instance = this;
        }

        [UsedImplicitly]
        public static OutlineWindowShowCommand Register(AsyncPackage package, OleMenuCommandService commandService) {
            return new OutlineWindowShowCommand(package, commandService);
        }

        protected override bool ShouldEnable() => true;

        public override void Execute() {

            Package.JoinableTaskFactory.RunAsync(async delegate {
                var window = await Package.ShowToolWindowAsync(typeof(GdOutlineToolWindowPane), id: 0, create: true, Package.DisposalToken);

                if (window?.Frame == null) {
                    throw new NotSupportedException("Cannot create tool window");
                }

                // Bug in Studio: Wenn das Fenster das erste mal erstellt wird, wird es nicht angezeigt...
                await Package.ShowToolWindowAsync(typeof(GdOutlineToolWindowPane), id: 0, create: false, Package.DisposalToken);
            });
        }


    }

}