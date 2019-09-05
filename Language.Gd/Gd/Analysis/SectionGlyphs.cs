using System;

using JetBrains.Annotations;

namespace Pharmatechnik.Language.Gd {

    public static class SectionGlyphs {

        public static Glyph GetGlyph([CanBeNull] ISectionSyntax guiElement) {

            // Container

            if (guiElement is DialogSectionSyntax) {
                return Glyph.Dialog;
            }

            if (guiElement is FormSectionSyntax) {
                return Glyph.Form;
            }

            if (guiElement is UserControlSectionSyntax) {
                return Glyph.UserControl;
            }

            // GuiControls

            if (guiElement is BarManagerSectionSyntax) {
                return Glyph.BarManager;
            }

            if (guiElement is DetailsPanelSectionSyntax) {
                return Glyph.DetailsPanel;
            }

            if (guiElement is MultiViewSectionSyntax) {
                return Glyph.MultiView;
            }

            if (guiElement is PanelSectionSyntax) {
                return Glyph.Panel;
            }

            if (guiElement is TabNavigationSectionSyntax) {
                return Glyph.TabNavigation;
            }

            // Sonderlocke TabPage
            if (guiElement is TabPageSectionSyntax) {
                return Glyph.TabPage;
            }

            if (guiElement is ControlSectionSyntax control) {

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