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
            ShowGuiOutlineButton.ToolTip = GetTooltipText(OutlineWindowShowCommand.Instance.CommandId,  "Gui Outline");
            GuiPreviewButton.ToolTip     = GetTooltipText(IxosEssentialsCommandIds.GuiPreviewCommandId, "Gui Preview");
            GenerateGuiButton.ToolTip    = GetTooltipText(IxosEssentialsCommandIds.GdGenerateCommandId, "C# Code aus .gd-Dateien generieren");
        }

        private void OnShowGuiOutlineClick(object sender, RoutedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            OutlineWindowShowCommand.Instance?.Execute();
            UpdateTooltips();
        }

        private void OnGenerateGuiButtonClick(object sender, RoutedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            GdGenerateSelectionCommand.Instance?.Execute();
            UpdateTooltips();
        }

        private void OnGuiPreviewClick(object sender, RoutedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            GdPreviewSelectionCommand.Instance?.Execute();
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

}