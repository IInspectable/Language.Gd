#region Using Directives

using System;
using System.ComponentModel.Design;

using Microsoft.VisualStudio.Shell;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

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

}