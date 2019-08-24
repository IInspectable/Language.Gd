using System.Threading;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    // TODO Parse Methods generieren..
    public static class Syntax {

        public static GuiDescriptionSyntax ParseGuiDescriptionSyntax(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {

            return (GuiDescriptionSyntax) SyntaxTree.Parse(SourceText.From(text)).Root;
        }

    }

}