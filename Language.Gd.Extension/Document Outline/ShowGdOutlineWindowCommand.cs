#region Using Directives

using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    // TODO ggf allgemeine Command Infrastruktur aufbauen
    internal sealed class ShowGdOutlineWindowCommand {

        readonly AsyncPackage _package;

        public static ShowGdOutlineWindowCommand Instance { get; private set; }

        public static readonly CommandID CommandId = new CommandID(PackageGuids.GdLanguagePackageCmdSetGuid, PackageIds.ShowGdOutlineWindow);

        private ShowGdOutlineWindowCommand(AsyncPackage package, OleMenuCommandService commandService) {

            _package       = package        ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuItem = new MenuCommand(OnExecute, CommandId);
            commandService.AddCommand(menuItem);

            Instance = this;
        }

        [UsedImplicitly]
        public static async Task<ShowGdOutlineWindowCommand> RegisterAsync(AsyncPackage package) {

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            return new ShowGdOutlineWindowCommand(package, commandService);
        }

        private void OnExecute(object sender, EventArgs e) {
            Execute();
        }

        public void Execute() {

            _package.JoinableTaskFactory.RunAsync(async delegate {
                var window = await _package.ShowToolWindowAsync(typeof(GdOutlineToolWindowPane), id: 0, create: true, _package.DisposalToken);

                if (window?.Frame == null) {
                    throw new NotSupportedException("Cannot create tool window");
                }

                // Bug in Studio: Wenn das Fenster das erste mal erstellt wird, wird es nicht angezeigt...
                await _package.ShowToolWindowAsync(typeof(GdOutlineToolWindowPane), id: 0, create: false, _package.DisposalToken);
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