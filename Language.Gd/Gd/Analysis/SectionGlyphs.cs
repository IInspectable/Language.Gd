using System;

using JetBrains.Annotations;

namespace Pharmatechnik.Language.Gd {

    public static class SectionGlyphs {

        public static Glyph GetGlyph([CanBeNull] ISectionSyntax section) {

            if (section is NamespaceDeclarationSectionSyntax) {
                return Glyph.Namespace;
            }
            // Container

            if (section is DialogSectionSyntax) {
                return Glyph.Dialog;
            }

            if (section is FormSectionSyntax) {
                return Glyph.Form;
            }

            if (section is UserControlSectionSyntax) {
                return Glyph.UserControl;
            }

            // GuiControls

            if (section is BarManagerSectionSyntax) {
                return Glyph.BarManager;
            }

            if (section is DetailsPanelSectionSyntax) {
                return Glyph.DetailsPanel;
            }

            if (section is MultiViewSectionSyntax) {
                return Glyph.MultiView;
            }

            if (section is PanelSectionSyntax) {
                return Glyph.Panel;
            }

            if (section is TabNavigationSectionSyntax) {
                return Glyph.TabNavigation;
            }

            // Sonderlocke TabPage
            if (section is TabPageSectionSyntax) {
                return Glyph.TabPage;
            }

            if (section is ControlSectionSyntax control) {

                var controlTypeText = control.ControlSectionBegin?.ControlTypeToken.Text;
                var controlType     = GetControlType(controlTypeText);

                return Enum.TryParse<Glyph>(controlType.ToString(), out var controlGlyph) ? controlGlyph : Glyph.None;

            }

            return default;
        }

        static ControlType GetControlType([CanBeNull] string controlTypeText) {
            return Enum.TryParse<ControlType>(controlTypeText, out var controlType) ? controlType : ControlType.None;

        }

    }

}