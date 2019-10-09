#region Using Directives

using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Language.Gd.DocumentOutline {

    public static class NavigatableSections {

        public static ImmutableArray<OutlineElement> GetItems(GuiDescriptionSyntax syntaxRoot) {

            var outlineElement = OutlineBuilder.Build(syntaxRoot, detailed: true);
            if (outlineElement == null) {
                return ImmutableArray<OutlineElement>.Empty;
            }

            return outlineElement.Flatten();

        }

    }

}