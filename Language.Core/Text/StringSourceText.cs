#region Using Directives

using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;

#endregion

namespace Pharmatechnik.Language.Text {

    sealed class StringSourceText: SourceText {

        readonly ReadOnlyMemory<char>      _memory;
        readonly Lazy<ImmutableArray<int>> _textLines;

        public StringSourceText(string text, string filePath) {

            _memory    = (text ?? String.Empty).AsMemory();
            _textLines = new Lazy<ImmutableArray<int>>(() => _memory.Span.ParseLineStarts(), LazyThreadSafetyMode.PublicationOnly);
            FileInfo   = String.IsNullOrEmpty(filePath) ? null : new FileInfo(filePath);
            TextLines  = new StringTextLineList(this);
        }

        public override FileInfo           FileInfo  { get; }
        public override SourceTextLineList TextLines { get; }
        public override string             Text      => _memory.ToString();
        public override int                Length    => _memory.Length;
        public override ReadOnlySpan<char> Span      => _memory.Span;

        public override char this[int index] => _memory.Span[index];
       
        public override ReadOnlySpan<char> Slice(int startIndex, int length) {
            return Span.Slice(start: startIndex, length: length);
        }

        SourceTextLine GetTextLine(int line, ImmutableArray<int> lineStarts) {

            if (line < 0 || line >= lineStarts.Length) {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            int start = lineStarts[line];
            if (line == lineStarts.Length - 1) {
                int end = Length;
                return new SourceTextLine(this, line: line, lineStart: start, lineEnd: end);
            } else {
                int end = lineStarts[line + 1];
                return new SourceTextLine(this, line: line, lineStart: start, lineEnd: end);
            }
        }

        sealed class StringTextLineList: SourceTextLineList {

            private readonly StringSourceText _sourceText;

            public StringTextLineList(StringSourceText sourceText) {
                _sourceText = sourceText;

            }

            public override int Count => _sourceText._textLines.Value.Length;

            public override SourceTextLine this[int index] => _sourceText.GetTextLine(index, _sourceText._textLines.Value);

        }

    }

}