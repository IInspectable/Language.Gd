#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.DocumentOutline {

    public class OutlineBuilder: SyntaxVisitor<OutlineElement> {

        [CanBeNull]
        public static OutlineElement Build(SyntaxNode node) {
            return Build(node as GuiDescriptionSyntax);
        }

        [CanBeNull]
        public static OutlineElement Build(GuiDescriptionSyntax syntaxRoot) {

            if (syntaxRoot == null) {
                return null;
            }

            var builder = new OutlineBuilder();

            // Startpunkt ist bisweilen immer der Container
            var container = syntaxRoot.DescendantNodes().OfType<ContainerSyntax>().FirstOrDefault();
            if (container == null) {
                return null;
            }

            return builder.Visit(container);
        }

        protected internal override OutlineElement VisitContainerSyntax(ContainerSyntax container) {

            var section = container as IContainerSyntax;

            return CreateSectionWithChildElements(container,
                                                  section?.ContainerDeclaration?.ControlsSection?.GuiElements,
                                                  section?.ContainerDeclaration?.NonVisualControlsSection?.ControlSections);
        }

        protected internal override OutlineElement VisitPanelSectionSyntax(PanelSectionSyntax panelSection) {
            return CreateSectionWithChildElements(panelSection, panelSection?.ControlsSection?.GuiElements);
        }

        protected internal override OutlineElement VisitDetailsPanelSectionSyntax(DetailsPanelSectionSyntax detailsPanelSection) {
            return CreateSectionWithChildElements(detailsPanelSection, detailsPanelSection.ControlsSection?.GuiElements);
        }

        protected internal override OutlineElement VisitGuiElementSyntax(GuiElementSyntax guiElement) {
            return CreateSectionElement(guiElement);
        }

        protected internal override OutlineElement VisitBarManagerSectionSyntax(BarManagerSectionSyntax barManagerSection) {
            return CreateSectionWithChildElements(barManagerSection, barManagerSection.ControlsSection?.GuiElements);
        }

        protected internal override OutlineElement VisitTabPageSectionSyntax(TabPageSectionSyntax tabPageSection) {
            return CreateSectionWithChildElements(tabPageSection, tabPageSection.ControlsSection?.GuiElements);
        }

        protected internal override OutlineElement VisitTabNavigationSectionSyntax(TabNavigationSectionSyntax tabNavigationSection) {
            return CreateSectionWithChildElements(tabNavigationSection,
                                                  tabNavigationSection.TabsSection?.TabPageSections,
                                                  tabNavigationSection.SharedControlSection?.ControlSections);
        }

        protected internal override OutlineElement VisitUserControlsSectionSyntax(UserControlsSectionSyntax userControlsSection) {
            return CreateSectionWithChildElements(userControlsSection, userControlsSection.ControlSections);
        }

        protected internal override OutlineElement VisitNonVisualControlsSectionSyntax(NonVisualControlsSectionSyntax nonVisualControlsSection) {
            return CreateSectionWithChildElements(nonVisualControlsSection, nonVisualControlsSection.ControlSections);
        }

        protected internal override OutlineElement VisitMultiViewSectionSyntax(MultiViewSectionSyntax multiViewSection) {
            return CreateSectionWithChildElements(multiViewSection, multiViewSection.UserControlsSection?.ControlSections);
        }

        [CanBeNull]
        OutlineElement CreateSectionWithChildElements(SyntaxNode sectionSyntax, params IEnumerable<SyntaxNode>[] childLists) {

            var children = ImmutableArray.CreateBuilder<OutlineElement>();

            foreach (var childElements in childLists) {

                if (childElements != null) {
                    foreach (var childElement in childElements) {
                        var child = Visit(childElement);
                        if (child != null) {
                            children.Add(child);
                        }
                    }
                }
            }

            return CreateSectionElement(sectionSyntax, children.ToImmutable());
        }

        [CanBeNull]
        OutlineElement CreateSectionElement(SyntaxNode sectionSyntax, ImmutableArray<OutlineElement> children = default) {

            var section      = sectionSyntax as ISectionSyntax;
            var sectionBegin = section?.SectionBegin;
            var sectionEnd   = section?.SectionEnd;

            if (sectionBegin == null || sectionBegin.IsMissing ||
                sectionEnd   == null || sectionEnd.IsMissing) {
                return null;
            }

            var displayName = GetDisplayName(sectionBegin);
            if (displayName.IsNullOrEmpty()) {
                return null;
            }

            var glyph = SectionGlyphs.GetGlyph(sectionSyntax);

            return new OutlineElement(displayName, sectionSyntax.FullExtent, sectionSyntax.ExtentStart, glyph, children);
        }

        [CanBeNull]
        string GetDisplayName(SyntaxNode node) {
            if (node == null) {
                return null;
            }

            var tokens = node.DescendantTokens().Select(t => t.Text);
            return String.Join(" ", tokens);
        }

    }

}