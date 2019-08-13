using Antlr4.Runtime;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    static class TextExtentFactory {
        
        public static TextExtent CreateExtent(IToken token) {
            return CreateExtent(token, token);
        }

        static TextExtent CreateExtent(IToken startToken, IToken endToken) {

            if (startToken == null || endToken == null) {
                return TextExtent.Missing;
            }

            if (startToken.StartIndex == -1 || endToken.StopIndex < -1) {
                return TextExtent.Missing;
            }

            var start = startToken.StartIndex;
            var end   = endToken.StopIndex + 1;
            // Warum auch immer Antlr so etwas absurdes liefert...
            if (end < start) {
                return TextExtent.Missing;
            }

            return TextExtent.FromBounds(
                start: start,
                end: end
            );
        }

    }

}