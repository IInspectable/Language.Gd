#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.ParserService {

    sealed class SyntaxTreeAndSnapshot : AndSnapshot {

        internal SyntaxTreeAndSnapshot([NotNull] SyntaxTree syntaxTree, ITextSnapshot snapshot) : base(snapshot) {
            SyntaxTree = syntaxTree;
        }

        public SyntaxTree SyntaxTree { get; }

    }
}