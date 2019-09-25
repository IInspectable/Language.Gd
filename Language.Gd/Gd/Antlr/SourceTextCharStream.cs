using System;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Antlr {

    sealed class SourceTextCharStream: ICharStream {

        readonly SourceText _sourceText;

        /// <summary>What is name or source of this char stream?</summary>
        public SourceTextCharStream(SourceText sourceText) {
            _sourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
        }

        /// <summary>
        /// Reset the stream so that it's in the same state it was
        /// when the object was created *except* the data array is not
        /// touched.
        /// </summary>
        public void Reset() {
            Index = 0;
        }

        public void Consume() {
            if (Index >= Size) {
                throw new InvalidOperationException("cannot consume EOF");
            }

            if (Index >= Size) {
                return;
            }

            ++Index;
        }

        public int La(int i) {
            if (i == 0) {
                return 0;
            }

            if (i < 0) {
                ++i;
                if (Index + i - 1 < 0) {
                    return -1;
                }
            }

            if (Index + i - 1 >= Size) {
                return -1;
            }

            return _sourceText[Index + i - 1];
        }

        public int Lt(int i) {
            return La(i);
        }

        /// <summary>
        /// Return the current input symbol index 0..n where n indicates the
        /// last symbol has been read.
        /// </summary>
        /// <remarks>
        /// Return the current input symbol index 0..n where n indicates the
        /// last symbol has been read.  The index is the index of char to
        /// be returned from LA(1).
        /// </remarks>
        public int Index { get; private set; }

        public int Size => _sourceText.Length;

        /// <summary>mark/release do nothing; we have entire buffer</summary>
        public int Mark() {
            return -1;
        }

        public void Release(int marker) {
        }

        /// <summary>
        /// consume() ahead until p==index; can't just set p=index as we must
        /// update line and charPositionInLine.
        /// </summary>
        /// <remarks>
        /// consume() ahead until p==index; can't just set p=index as we must
        /// update line and charPositionInLine. If we seek backwards, just set p
        /// </remarks>
        public void Seek(int index) {
            if (index <= Index) {
                Index = index;
            } else {
                index = Math.Min(index, Size);
                while (Index < index) {
                    Consume();
                }
            }
        }

        public string GetText(Interval interval) {
            int a   = interval.a;
            int num = interval.b;
            if (num >= Size) {
                num = Size - 1;
            }

            int length = num - a + 1;
            if (a >= Size) {
                return string.Empty;
            }

            return _sourceText.Substring(a, length);
        }

        public string SourceName {
            get {
                var fileName = _sourceText.FileInfo?.FullName;
                if (fileName.IsNullOrEmpty()) {
                    return "<unknown>";
                }

                return fileName;
            }
        }

    }

}