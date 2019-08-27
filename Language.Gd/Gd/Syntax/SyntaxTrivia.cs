using System;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    public readonly struct SyntaxTrivia {

        private readonly TriviaSlot _slot;

        internal SyntaxTrivia(SyntaxTree syntaxTree, SyntaxToken token, TriviaSlot slot, int position) {
            Position   = position;
            SyntaxTree = syntaxTree;
            Token      = token;
            _slot      = slot;
        }

        [CanBeNull]
        public SyntaxTree SyntaxTree { get; }

        public SyntaxToken Token { get; }

        public SyntaxKind Kind => _slot.Kind;

        public bool IsMissing           => _slot.IsMissing;
        public bool IsSkipedTokenTrivia => _slot.IsSkipedTokenTrivia;

        internal int Position    { get; }
        internal int EndPosition => Position + _slot.FullLength;

        public int Start      => Position + _slot.GetLeadingTriviaWidth();
        public int Length     => _slot.Length;
        public int FullLength => _slot.FullLength;
        public int End        => Position + _slot.FullLength;

        public TextExtent FullExtent => new TextExtent(start: Position, length: FullLength);
        public TextExtent Extent     => new TextExtent(start: Start,    length: Length);

        public string GetText() => SyntaxTree?.SourceText.Substring(Extent) ?? String.Empty;

        public Location GetLocation() {
            return SyntaxTree?.GetLocation(Extent) ?? Location.None;
        }

        public override string ToString() {
            return $"Tr: {Kind}: {GetText()}";
        }

    }

}