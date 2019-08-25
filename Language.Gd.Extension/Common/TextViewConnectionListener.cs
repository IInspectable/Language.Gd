#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    [Export(typeof(IWpfTextViewConnectionListener))]
    [Export(typeof(TextViewConnectionListener))]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    sealed class TextViewConnectionListener: IWpfTextViewConnectionListener {

        readonly Dictionary<IWpfTextView, List<Action<IWpfTextView>>> _textViews;

        public TextViewConnectionListener() {
            _textViews = new Dictionary<IWpfTextView, List<Action<IWpfTextView>>>();
        }

        public void SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers) {
            _textViews[textView] = new List<Action<IWpfTextView>>();
        }

        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers) {
            var actions = _textViews[textView];

            _textViews.Remove(textView);

            foreach (var action in actions) {
                action(textView);
            }
        }

        public IWpfTextView GetTextViewForBuffer(ITextBuffer textBuffer) {
            return _textViews.Keys.FirstOrDefault(t => t.TextBuffer == textBuffer);
        }

        public void AddDisconnectAction(IWpfTextView textView, Action<IWpfTextView> action) {
            _textViews[textView].Add(action);
        }

    }

}