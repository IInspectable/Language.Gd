#region Using Directives

using System;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class GdOutlineEventArgs: EventArgs {

        public GdOutlineEventArgs(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot, int? activePosition) {
            SyntaxTreeAndSnapshot = syntaxTreeAndSnapshot;
            ActivePosition        = activePosition;

        }

        [CanBeNull]
        public SyntaxTreeAndSnapshot SyntaxTreeAndSnapshot { get; }

        public int? ActivePosition { get; }

    }

}