#region Using Directives

using System;
using System.Windows;
using System.ComponentModel.Design;

using EnvDTE;

using JetBrains.Annotations;

using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.Extension.Document_Outline;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Margin {

    public partial class GdMarginControl {

        public GdMarginControl() {
            InitializeComponent();

            UpdateTooltips();
        }

        void UpdateTooltips() {
            ShowGuiOutlineButton.ToolTip = GetTooltipText(ShowGdOutlineWindowCommand.CommandId, "Gui Outline");
            GuiPreviewButton.ToolTip     = GetTooltipText(Ids.GuiPreviewCommandId,              "Gui Preview");
            GenerateGuiButton.ToolTip    = GetTooltipText(Ids.GdGenerateCommandCommandId,       "C# Code aus .gd-Dateien generieren");
        }

        private void OnShowGuiOutlineClick(object sender, RoutedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            InvokeCommand(ShowGdOutlineWindowCommand.CommandId);
            //ShowGdOutlineWindowCommand.Instance.Execute();
        }

        private void OnGenerateGuiButtonClick(object sender, RoutedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            InvokeCommand(Ids.GdGenerateCommandCommandId);
        }

        private void OnGuiPreviewClick(object sender, RoutedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            InvokeCommand(Ids.GuiPreviewCommandId);
        }

        [CanBeNull]
        static Command TryGetCommand(CommandID commandId) {

            try {
                ThreadHelper.ThrowIfNotOnUIThread();
                return GdLanguagePackage.DTE.Commands.Item(commandId.Guid.ToString("B"), commandId.ID);
            } catch (ArgumentException) {

            }

            return null;
        }

        // TODO Geht das nicht schöner?
        private static void InvokeCommand(CommandID commandId) {
            ThreadHelper.ThrowIfNotOnUIThread();
            var cmd = TryGetCommand(commandId);

            if (cmd != null && cmd.IsAvailable) {
                GdLanguagePackage.DTE.Commands.Raise(cmd.Guid, cmd.ID, null, null);
                GdLanguagePackage.DTE.DTE.StatusBar.Clear();
            } else {
                GdLanguagePackage.DTE.DTE.StatusBar.Text = $"The command '{cmd?.Name}' is not available in the current context";
            }

        }

        static string GetTooltipText(CommandID commandId, string commandName) {

            var tooltipText = commandName;

            var keyBinding = KeyBindingHelper.GetGlobalKeyBinding(commandId.Guid, commandId.ID);
            if (!String.IsNullOrEmpty(keyBinding)) {
                tooltipText += $" ({keyBinding})";
            }

            return tooltipText;
        }

        // IDS siehe IXOS Essentials
        static class Ids {

            const int GdGenerateCommand         = 0x0251;
            const int GdPreviewSelectionCommand = 0x0253;

            const  string GuidIXOSEssentialsPackageCmdSetString = "6b794c4c-4923-45f3-a677-8cfca59df62f";
            static Guid   GuidIXOSEssentialsPackageCmdSet       = new Guid(GuidIXOSEssentialsPackageCmdSetString);

            public static CommandID GdGenerateCommandCommandId => new CommandID(GuidIXOSEssentialsPackageCmdSet, GdGenerateCommand);

            // TODO
            public static CommandID GuiPreviewCommandId => new CommandID(GuidIXOSEssentialsPackageCmdSet, GdPreviewSelectionCommand);

        }

    }

}