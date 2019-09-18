#region Using Directives

using System;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text;

using Pharmatechnik.Language.Gd.DocumentOutline;
using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class OutlineData: SyntaxTreeAndSnapshot {

        public OutlineData(OutlineElement outlineElement,
                           int? activePosition,
                           [NotNull] SyntaxTree syntaxTree,
                           [NotNull] ITextSnapshot snapshot): base(syntaxTree, snapshot) {
            OutlineElement = outlineElement;
            ActivePosition = activePosition;
        }

        public OutlineElement OutlineElement { get; }

        public int? ActivePosition { get; }

    }

    class GdOutlineEventArgs: EventArgs {

        public GdOutlineEventArgs(OutlineData outlineData) {
            OutlineData = outlineData;

        }

        [CanBeNull]
        public OutlineData OutlineData { get; }

    }

}