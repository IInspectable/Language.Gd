#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Language.Text;
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
            var sectionEnd   = section?.SectionEnd;

            if (sectionBegin == null || sectionBegin.IsMissing ||
                sectionEnd   == null || sectionEnd.IsMissing) {
                return;
            }

            var displayName = GetDisplayName(sectionBegin);
            if (displayName.IsNullOrEmpty()) {
                return;
            }

            NavigationItems.Add(new NavigationBarItem(
                                    displayName    : displayName,
                                    imageMoniker   : GetImageMoniker(sectionSyntax),
                                    extent         : sectionSyntax.FullExtent,
                                    navigationPoint: sectionSyntax.ExtentStart));
        }

        string GetDisplayName(SyntaxNode node) {
            var tokens = node.DescendantTokens().Select(t => t.Text);
            return String.Join(" ", tokens);
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

            if (guiElement is TabNavigationSectionSyntax) {
                return KnownMonikers.Tab;
            }

            if (guiElement is ControlSectionSyntax control) {

                switch (GetControlType(control.ControlSectionBegin?.ControlTypeToken.Text)) {

                    case ControlType.None:
                        return KnownMonikers.None;
                    case ControlType.AmountTextbox:
                        return KnownMonikers.Currency;
                    case ControlType.BrowsableTextbox:
                        return KnownMonikers.FilteredTextBox;
                    case ControlType.Button:
                        return KnownMonikers.Button;
                    case ControlType.ButtonBarManager:
                        return KnownMonikers.ApplicationBar;
                    case ControlType.Cave:
                        return KnownMonikers.InteractionUse;
                    case ControlType.Chart:
                        return KnownMonikers.ColumnChart;
                    case ControlType.Checkbox:
                        return KnownMonikers.CheckBoxChecked;
                    case ControlType.Combobox:
                        return KnownMonikers.ComboBox;
                    case ControlType.Control:
                        return KnownMonikers.Control;
                    case ControlType.DateTextbox:
                        return KnownMonikers.DateTimePicker;
                    case ControlType.DetailsPanel:
                        return KnownMonikers.DetailView;
                    case ControlType.Dropdownbox:
                        return KnownMonikers.ComboBox;
                    case ControlType.DynamicButton:
                        return KnownMonikers.Button;
                    case ControlType.DynamicFunctionButton:
                        return KnownMonikers.Button;
                    case ControlType.DynamicLabel:
                        return KnownMonikers.Label;
                    case ControlType.ExplorerBar:
                        return KnownMonikers.ToolstripPanelLeft;
                    case ControlType.FormattedLabel:
                        return KnownMonikers.TextBlock;
                    case ControlType.FormattedTextbox:
                        return KnownMonikers.RichTextBox;
                    case ControlType.FunctionButton:
                        return KnownMonikers.Button;
                    case ControlType.FunctionButtonBar:
                        return KnownMonikers.ApplicationBar;
                    case ControlType.HeaderScroller:
                        return KnownMonikers.VerticalScrollBar;
                    case ControlType.Label:
                        return KnownMonikers.Label;
                    case ControlType.Listbox:
                        return KnownMonikers.ListBox;
                    case ControlType.MaskTextbox:
                        return KnownMonikers.MaskedTextBox;
                    case ControlType.MultiViewContainer:
                        return KnownMonikers.MultiView;
                    case ControlType.NumericTextbox:
                        return KnownMonikers.PhoneNumberEditor;
                    case ControlType.Panel:
                        return KnownMonikers.Panel;
                    case ControlType.Picturebox:
                    case ControlType.PersistentPictureBox:
                        return KnownMonikers.PictureAndText;
                    case ControlType.Radiobutton:
                        return KnownMonikers.RadioButton;
                    case ControlType.RadiobuttonGroup:
                        return KnownMonikers.RadioButtonList;
                    case ControlType.ReportPreview:
                        return KnownMonikers.Report;
                    case ControlType.Scanner:
                        return KnownMonikers.InfraredDevice;
                    case ControlType.TabPage:
                    case ControlType.TabNavigation:
                        return KnownMonikers.Tab;
                    case ControlType.Table:
                        return KnownMonikers.Table;
                    case ControlType.Textbox:
                        return KnownMonikers.TextBox;
                    case ControlType.TimeTextbox:
                        return KnownMonikers.TimePicker;
                    case ControlType.Tree:
                        return KnownMonikers.TreeView;
                    case ControlType.UserControlReference:
                        return KnownMonikers.InheritedUserControl;
                    case ControlType.WebBrowser:
                        return KnownMonikers.WebBrowserItem;
                    case ControlType.SelectionList:
                        break;
                    case ControlType.ExtendedControl:
                        break;
                    case ControlType.TabStrip:
                        break;
                }

                return KnownMonikers.Control;
            }

            return default;
        }

        ControlType GetControlType(string controlTypeText) {
            return Enum.TryParse<ControlType>(controlTypeText, out var controlType) ? controlType : ControlType.None;

        }

        #if ShowMemberCombobox
        public override void VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            MemberItems.Add(new NavigationBarItem(signalTriggerSymbol.Name, NavigationBarImages.Index.TriggerSymbol, signalTriggerSymbol.Transition.Location, signalTriggerSymbol.Start));
        }
        #endif

    }

    public enum ControlType {

        None,
        AmountTextbox,
        BrowsableTextbox,
        Button,
        ButtonBarManager,
        Cave,
        Chart,
        Checkbox,
        Combobox,
        Control,
        DateTextbox,
        DetailsPanel,
        Dropdownbox,
        DynamicButton,
        DynamicFunctionButton,
        DynamicLabel,
        ExplorerBar,
        ExtendedControl,
        FormattedLabel,
        FormattedTextbox,
        FunctionButton,
        FunctionButtonBar,
        HeaderScroller,
        Label,
        Listbox,
        MaskTextbox,
        MultiViewContainer,
        NumericTextbox,
        Panel,
        PersistentPictureBox,
        Picturebox,
        Radiobutton,
        RadiobuttonGroup,
        ReportPreview,
        Scanner,
        SelectionList,
        TabNavigation,
        TabPage,
        TabStrip,
        Table,
        Textbox,
        TimeTextbox,
        Tree,
        UserControlReference,
        WebBrowser,

    }

}