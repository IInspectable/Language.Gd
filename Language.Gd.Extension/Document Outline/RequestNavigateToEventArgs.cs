#region Using Directives

using System;

using Pharmatechnik.Language.Gd.DocumentOutline;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    public class RequestNavigateToEventArgs: EventArgs {

        public RequestNavigateToEventArgs(OutlineElement outlineElement) {
            OutlineElement = outlineElement;

        }

        public OutlineElement OutlineElement { get; }

    }

}