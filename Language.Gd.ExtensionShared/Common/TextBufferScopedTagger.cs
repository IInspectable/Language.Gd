#region Using Directives

using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    sealed class TextBufferScopedTagger<TTag>: ITagger<TTag>, IDisposable
        where TTag : ITag {

        readonly TextBufferScopedValue<ITagger<TTag>> _textBufferScopedTagger;

        internal TextBufferScopedTagger(
            ITextBuffer textBuffer,
            object key,
            Func<ITagger<TTag>> createFunc) {
            _textBufferScopedTagger = TextBufferScopedValue<ITagger<TTag>>.GetOrCreate(textBuffer, key, createFunc);
        }

        ITagger<TTag> Tagger {
            get { return _textBufferScopedTagger.Value; }
        }

        public void Dispose() {
            _textBufferScopedTagger.Dispose();
        }

        IEnumerable<ITagSpan<TTag>> ITagger<TTag>.GetTags(NormalizedSnapshotSpanCollection col) {
            return Tagger.GetTags(col);
        }

        event EventHandler<SnapshotSpanEventArgs> ITagger<TTag>.TagsChanged {
            add { Tagger.TagsChanged    += value; }
            remove { Tagger.TagsChanged -= value; }
        }

    }

}