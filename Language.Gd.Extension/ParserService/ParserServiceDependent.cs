#region Using Directives

using System;

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.ParserService {

    abstract class ParserServiceDependent: IDisposable {

        protected ParserServiceDependent(ITextBuffer textBuffer) {

            TextBuffer = textBuffer;

            ParserService = ParserService.GetOrCreateSingelton(textBuffer);

            ParserService.ParseResultChanging += OnParseResultChanging;
            ParserService.ParseResultChanged  += OnParseResultChanged;
        }

        public virtual void Dispose() {
            ParserService.ParseResultChanging -= OnParseResultChanging;
            ParserService.ParseResultChanged  -= OnParseResultChanged;
        }

        protected ITextBuffer   TextBuffer    { get; }
        protected ParserService ParserService { get; }

        protected virtual void OnParseResultChanging(object sender, EventArgs e) {
        }

        protected virtual void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
        }

    }

}