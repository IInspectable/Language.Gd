using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Pharmatechnik.Language.Gd.Extension.SmartIndent {

    [ContentType(GdLanguageContentDefinitions.ContentType)]
    [Export(typeof(ISmartIndentProvider))]
    class GdSmartIndentProvider: ISmartIndentProvider {

        public ISmartIndent CreateSmartIndent(ITextView textView) => new GdSmartIndent(textView);

    }

}