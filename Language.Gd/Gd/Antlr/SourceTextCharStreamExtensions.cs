using Antlr4.Runtime;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Antlr {

    static class SourceTextCharStreamExtensions {

        public static ICharStream ToCharStream(this SourceText sourceText) {
            return new SourceTextCharStream(sourceText);
        }

    }

}