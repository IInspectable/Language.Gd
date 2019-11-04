#region Using Directives

using System;
using System.Windows;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Controls;

using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Language.Gd.Extension.Document_Outline;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Margin {

    public partial class GdMarginControl {

        public GdMarginControl(IWpfTextView textView) {
            InitializeComponent();

            UpdateTooltips();

            var buttonStyle = (Style) textView.VisualElement.TryFindResource("FileHealthIndicatorButtonStyle");
            if (buttonStyle != null) {
                foreach (var button in LayoutPanel.Children.OfType<Button>()) {
                    button.Style = buttonStyle;
                }
            }

        }

        void UpdateTooltips() {
            ShowGuiOutlineButton.ToolTip = GetTooltipText(ShowGdOutlineWindowCommand.Instance.CommandId, "Gui Outline");
            GuiPreviewButton.ToolTip     = GetTooltipText(IxosEssentialsCommandIds.GuiPreviewCommandId,  "Gui Preview");
            GenerateGuiButton.ToolTip    = GetTooltipText(IxosEssentialsCommandIds.GdGenerateCommandId,  "C# Code aus .gd-Dateien generieren");
        }

        private void OnShowGuiOutlineClick(object sender, RoutedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            InvokeCommand(ShowGdOutlineWindowCommand.Instance.CommandId);
            //ShowGdOutlineWindowCommand.Instance.Execute();
        }

        private void OnGenerateGuiButtonClick(object sender, RoutedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            InvokeCommand(IxosEssentialsCommandIds.GdGenerateCommandId);
        }

        private void OnGuiPreviewClick(object sender, RoutedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            InvokeCommand(IxosEssentialsCommandIds.GuiPreviewCommandId);
        }

        //[CanBeNull]
        //static Command TryGetCommand(CommandID commandId) {

        //    try {
        //        ThreadHelper.ThrowIfNotOnUIThread();
        //        return GdLanguagePackage.DTE.Commands.Item(commandId.Guid.ToString("B"), commandId.ID);
        //    } catch (ArgumentException) {

        //    }

        //    return null;
        //}

        private void InvokeCommand(CommandID commandId) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var oleCommandTarget = GdLanguagePackage.ServiceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;

            oleCommandTarget?.GlobalInvoke(commandId);

            UpdateTooltips();

        }

        static string GetTooltipText(CommandID commandId, string commandName) {

            var tooltipText = commandName;

            var keyBinding = KeyBindingHelper.GetGlobalKeyBinding(commandId.Guid, commandId.ID);
            if (!String.IsNullOrEmpty(keyBinding)) {
                tooltipText += $" ({keyBinding})";
            }

            return tooltipText;
        }

    }

    // IDS siehe IXOS Essentials
    static class IxosEssentialsCommandIds {

        const int GdGenerateSelectionCommand = 0x0252;
        const int GdPreviewSelectionCommand  = 0x0253;

        const           string GuidIXOSEssentialsPackageCmdSetString = "6b794c4c-4923-45f3-a677-8cfca59df62f";
        static readonly Guid   GuidIXOSEssentialsPackageCmdSet       = new Guid(GuidIXOSEssentialsPackageCmdSetString);

        public static CommandID GdGenerateCommandId => new CommandID(GuidIXOSEssentialsPackageCmdSet, GdGenerateSelectionCommand);
        public static CommandID GuiPreviewCommandId => new CommandID(GuidIXOSEssentialsPackageCmdSet, GdPreviewSelectionCommand);

    }

}