using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.DocumentOutline;

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class NavigateToOutlineEventArgs {

        public NavigateToOutlineEventArgs([CanBeNull] OutlineElement outlineElement) {
            OutlineElement = outlineElement;

        }

        [CanBeNull]
        public OutlineElement OutlineElement { get; }

    }

}