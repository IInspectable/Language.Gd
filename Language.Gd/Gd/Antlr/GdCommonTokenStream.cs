using System.Collections.Generic;

using Antlr4.Runtime;

namespace Pharmatechnik.Language.Gd.Antlr {

    sealed class GdCommonTokenStream : CommonTokenStream {

        public GdCommonTokenStream(ITokenSource tokenSource) : base(tokenSource) {
        }

        public GdCommonTokenStream(ITokenSource tokenSource, int channel) : base(tokenSource, channel) {
        }
       
        public IList<IToken> AllTokens => tokens;
    }

}