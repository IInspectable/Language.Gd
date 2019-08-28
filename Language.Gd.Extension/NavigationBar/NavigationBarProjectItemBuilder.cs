#region Using Directives

using System.Collections.Immutable;

using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Gd.Extension.ParserService;
using Pharmatechnik.Language.Gd.Extension.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.NavigationBar {

    class NavigationBarProjectItemBuilder {

        public static ImmutableList<NavigationBarItem> Build(SyntaxTreeAndSnapshot codeGenerationUnitAndSnapshot) {

            if (codeGenerationUnitAndSnapshot == null) {
                return ImmutableList<NavigationBarItem>.Empty;
            }

            return new[] {
                new NavigationBarItem(
                    displayName: codeGenerationUnitAndSnapshot.Snapshot.TextBuffer.GetContainingProject()?.Name ?? ProjectMapper.MiscellaneousFiles,
                    imageIndex: NavigationBarImages.Index.ProjectNode)
            }.ToImmutableList();
        }

    }

}