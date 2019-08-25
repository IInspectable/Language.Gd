#region Using Directives

using System;
using System.Windows;

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.ParserService {

    abstract class ParserServiceDependent {

        readonly ParserService _parserService;

        protected ParserServiceDependent(ITextBuffer textBuffer) {

            TextBuffer = textBuffer;

            _parserService = ParserService.GetOrCreateSingelton(textBuffer);

            WeakEventManager<ParserService, EventArgs>.AddHandler(_parserService, nameof(ParserService.ParseResultChanging), OnParseResultChanging);
            WeakEventManager<ParserService, SnapshotSpanEventArgs>.AddHandler(_parserService, nameof(ParserService.ParseResultChanged), OnParseResultChanged);
        }

        protected ITextBuffer TextBuffer { get; }
        protected ParserService ParserService => _parserService;

        protected virtual void OnParseResultChanging(object sender, EventArgs e) {
        }

        protected virtual void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
        }

    }

}