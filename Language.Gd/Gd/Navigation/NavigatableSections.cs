#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Navigation {

    public class NavigatableSections {

        public static ImmutableList<NavigationItem> GetItems(SyntaxNode syntaxRoot) {

            if (syntaxRoot == null) {
                return ImmutableList<NavigationItem>.Empty;
            }

            var builder = new Builder();
            builder.Visit(syntaxRoot);

            var items = builder.NavigationItems
                               .OrderBy(ni => ni.Start)
                               .ToImmutableList();

            return items;

        }

        class Builder: SyntaxListener {

            internal Builder() {
                NavigationItems = new List<NavigationItem>();
            }

            public List<NavigationItem> NavigationItems { get; }

            protected internal override void VisitContainerSyntax(ContainerSyntax container) {
                AddSection(container);
                DefaultVisit(container);
            }

            protected internal override void VisitGuiElementSyntax(GuiElementSyntax guiElement) {
                AddSection(guiElement);
                DefaultVisit(guiElement);
            }

            protected internal override void VisitTabPageSectionSyntax(TabPageSectionSyntax tabPageSection) {
                AddSection(tabPageSection);
                DefaultVisit(tabPageSection);
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

                var glyph = SectionGlyphs.GetGlyph(section);

                NavigationItems.Add(new NavigationItem(
                                        displayName    : displayName,
                                        glyph          : glyph,
                                        extent         : sectionSyntax.FullExtent,
                                        navigationPoint: sectionSyntax.ExtentStart));
            }

            string GetDisplayName(SyntaxNode node) {
                var tokens = node.DescendantTokens().Select(t => t.Text);
                return String.Join(" ", tokens);
            }

        }

    }

}