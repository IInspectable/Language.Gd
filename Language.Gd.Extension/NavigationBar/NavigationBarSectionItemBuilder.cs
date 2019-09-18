#region Using Directives

using System.Collections.Immutable;
using System.Linq;

using Pharmatechnik.Language.Gd.DocumentOutline;
using Pharmatechnik.Language.Gd.Extension.Imaging;
using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.NavigationBar {

    static class NavigationBarSectionItemBuilder {

        public static ImmutableList<NavigationBarItem> Build(SyntaxTreeAndSnapshot codeGenerationUnitAndSnapshot) {

            var syntaxRoot = codeGenerationUnitAndSnapshot?.SyntaxTree.Root as GuiDescriptionSyntax;

            return NavigatableSections.GetItems(syntaxRoot)
                                      .Select(item => new NavigationBarItem(
                                                  displayName    : item.DisplayName,
                                                  imageMoniker   : GdImageMonikers.GetMoniker(item.Glyph),
                                                  extent         : item.Extent,
                                                  navigationPoint: item.NavigationPoint))
                                      .ToImmutableList();

        }

    }

}