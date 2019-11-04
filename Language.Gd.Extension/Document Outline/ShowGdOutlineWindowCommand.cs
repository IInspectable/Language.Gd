#region Using Directives

using System;
using System.ComponentModel.Design;

using JetBrains.Annotations;

using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    internal abstract class Command {

        protected Command(AsyncPackage package, OleMenuCommandService commandService, int commandID) {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            Package        = package        ?? throw new ArgumentNullException(nameof(package));
            CommandId      = new CommandID(PackageGuids.GdLanguagePackageCmdSetGuid, commandID);

            var menuItem = new OleMenuCommand(OnExecute, CommandId, queryStatusSupported: true);

            menuItem.BeforeQueryStatus += OnBeforeQueryStatus;

            commandService.AddCommand(menuItem);

        }

        private void OnBeforeQueryStatus(object sender, EventArgs e) {

            var menuCommand = sender as OleMenuCommand;
            if (menuCommand == null) {
                return;
            }

            menuCommand.Enabled = ShouldEnable();
        }

        public AsyncPackage Package   { get; }
        public CommandID    CommandId { get; }

        private void OnExecute(object sender, EventArgs e) {
            Execute();
        }

        public abstract void Execute();

        protected abstract bool ShouldEnable();

    }

    internal sealed class ShowGdOutlineWindowCommand: Command {

        public static ShowGdOutlineWindowCommand Instance { get; private set; }

        private ShowGdOutlineWindowCommand(AsyncPackage package, OleMenuCommandService commandService):
            base(package, commandService,
                 PackageIds.ShowGdOutlineWindow) {

            Instance = this;
        }

        [UsedImplicitly]
        public static ShowGdOutlineWindowCommand Register(AsyncPackage package, OleMenuCommandService commandService) {
            return new ShowGdOutlineWindowCommand(package, commandService);
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

        public string GetTooltipText() {

            var tooltipText = "Gui Outline";

            var keyBinding = KeyBindingHelper.GetGlobalKeyBinding(PackageGuids.GdLanguagePackageCmdSetGuid, PackageIds.ShowGdOutlineWindow);
            if (!String.IsNullOrEmpty(keyBinding)) {
                tooltipText += $" ({keyBinding})";
            }

            return tooltipText;
        }

    }

}