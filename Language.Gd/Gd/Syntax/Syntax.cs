using System.Threading;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    // TODO Parse Methods generieren..
    public static class Syntax {

        public static GuiDescriptionSyntax ParseGuiDescriptionSyntax(string text, CancellationToken cancellationToken = default) {
            return ParseGuiDescriptionSyntax(SourceText.From(text), cancellationToken);
        }

        public static GuiDescriptionSyntax ParseGuiDescriptionSyntax(SourceText sourceText, CancellationToken cancellationToken = default) {
            return (GuiDescriptionSyntax) SyntaxTree.Parse(sourceText, treeCreator: parser => parser.guiDescription(), cancellationToken).Root;
        }

    }

}