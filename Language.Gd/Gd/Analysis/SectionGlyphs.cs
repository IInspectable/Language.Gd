using System;

using JetBrains.Annotations;

namespace Pharmatechnik.Language.Gd {

    public static class SectionGlyphs {

        public static Glyph GetGlyph([CanBeNull] SyntaxNode section) {

            if (section is NamespaceDeclarationSectionSyntax || section is NamespaceDeclarationSectionBeginSyntax) {
                return Glyph.Namespace;
            }

            // Container

            if (section is DialogSectionSyntax || section is DialogSectionBeginSyntax) {
                return Glyph.Dialog;
            }

            if (section is FormSectionSyntax || section is FormSectionBeginSyntax) {
                return Glyph.Form;
            }

            if (section is UserControlSectionSyntax || section is UserControlSectionBeginSyntax) {
                return Glyph.UserControl;
            }

            // GuiControls

            if (section is BarManagerSectionSyntax || section is BarManagerSectionBeginSyntax) {
                return Glyph.BarManager;
            }

            if (section is DetailsPanelSectionSyntax || section is DetailsPanelSectionBeginSyntax) {
                return Glyph.DetailsPanel;
            }

            if (section is MultiViewSectionSyntax || section is MultiViewSectionBeginSyntax) {
                return Glyph.MultiView;
            }

            if (section is PanelSectionSyntax || section is PanelSectionBeginSyntax) {
                return Glyph.Panel;
            }

            if (section is TabNavigationSectionSyntax || section is TabNavigationSectionBeginSyntax) {
                return Glyph.TabNavigation;
            }

            // Sonderlocke TabPage
            if (section is TabPageSectionSyntax || section is TabPageSectionBeginSyntax) {
                return Glyph.TabPage;
            }

            if (section is ControlSectionSyntax control) {

                var controlTypeText = control.ControlSectionBegin?.ControlTypeToken.GetText();

                return Enum.TryParse<Glyph>(controlTypeText, out var controlGlyph) ? controlGlyph : Glyph.None;

            }

            return default;
        }

    }

}