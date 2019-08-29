﻿#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.NavigationBar {

    class NavigationBarTaskItemBuilder: SyntaxListener {

        protected NavigationBarTaskItemBuilder() {
            NavigationItems = new List<NavigationBarItem>();
            MemberItems     = new List<NavigationBarItem>();
        }

        public List<NavigationBarItem> NavigationItems { get; }
        public List<NavigationBarItem> MemberItems     { get; }

        public static ImmutableList<NavigationBarItem> Build(SyntaxTreeAndSnapshot codeGenerationUnitAndSnapshot) {

            var syntaxRoot = codeGenerationUnitAndSnapshot?.SyntaxTree.Root;
            if (syntaxRoot == null) {
                return ImmutableList<NavigationBarItem>.Empty;
            }

            var builder = new NavigationBarTaskItemBuilder();
            builder.Visit(syntaxRoot);

            var items = builder.NavigationItems
                               .OrderBy(ni => ni.Start)
                               .ToImmutableList();

            return items;

        }

        protected override void VisitContainerSyntax(ContainerSyntax container) {
            AddSection(container);
            DefaultVisit(container);
        }

        protected override void VisitGuiElementSyntax(GuiElementSyntax guiElement) {
            AddSection(guiElement);
            DefaultVisit(guiElement);
        }

        private void AddSection(SyntaxNode sectionSyntax) {

            var section      = sectionSyntax as ISectionSyntax;
            var sectionBegin = section?.SectionBegin;

            if (sectionBegin == null) {
                return;
            }

            NavigationItems.Add(new NavigationBarItem(
                                    displayName: sectionBegin.GetText(),
                                    imageMoniker: GetImageMoniker(sectionSyntax),
                                    extent: sectionSyntax.FullExtent,
                                    navigationPoint: sectionSyntax.ExtentStart));
        }

        ImageMoniker GetImageMoniker(SyntaxNode guiElement) {

            // TODO Bisweilen nur pseudo Code

            if (guiElement is DialogSectionSyntax) {
                return KnownMonikers.Dialog;
            }

            if (guiElement is FormSectionSyntax) {
                return KnownMonikers.Dialog;
            }

            if (guiElement is UserControlSectionSyntax) {
                return KnownMonikers.UserControl;
            }

            if (guiElement is PanelSectionSyntax) {
                return KnownMonikers.Panel;
            }

            if (guiElement is BarManagerSectionSyntax) {
                return KnownMonikers.ApplicationBar;
            }

            if (guiElement is MultiViewSectionSyntax) {
                return KnownMonikers.MultiView;
            }

            if (guiElement is ControlSectionSyntax control) {

                switch (control.ControlSectionBegin?.ControlTypeToken.Text) {
                    case "Label":                 return KnownMonikers.Label;
                    case "UserControlReference":  return KnownMonikers.UserControl;
                    case "PersistentPictureBox":  return KnownMonikers.PictureAndText;
                    case "DynamicLabel":          return KnownMonikers.Label;
                    case "Button":                return KnownMonikers.Button;
                    case "DynamicButton":         return KnownMonikers.Button;
                    case "DateTextbox":           return KnownMonikers.DateTimePicker;
                    case "MaskTextbox":           return KnownMonikers.MaskedTextBox;
                    case "Textbox":               return KnownMonikers.TextBox;
                    case "FunctionButton":        return KnownMonikers.Button;
                    case "DynamicFunctionButton": return KnownMonikers.Button;
                    case "Table":                 return KnownMonikers.Table;
                }

                return KnownMonikers.Control;
            }

            return KnownMonikers.None;
        }

        #if ShowMemberCombobox
        public override void VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            MemberItems.Add(new NavigationBarItem(signalTriggerSymbol.Name, NavigationBarImages.Index.TriggerSymbol, signalTriggerSymbol.Transition.Location, signalTriggerSymbol.Start));
        }
        #endif

    }

}