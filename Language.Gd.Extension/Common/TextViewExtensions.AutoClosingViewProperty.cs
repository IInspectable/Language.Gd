#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static partial class TextViewExtensions {

        class AutoClosingViewProperty<TProperty, TTextView> where TTextView : ITextView {

            readonly TTextView                     _textView;
            readonly Dictionary<object, TProperty> _map = new Dictionary<object, TProperty>();

            public static bool GetOrCreateValue(
                TTextView textView,
                object key,
                Func<TTextView, TProperty> valueCreator,
                out TProperty value) {
                if (textView.IsClosed) {
                    throw new InvalidOperationException();
                }

                var properties = textView.Properties.GetOrCreateSingletonProperty(() => new AutoClosingViewProperty<TProperty, TTextView>(textView));
                if (!properties.TryGetValue(key, out value)) {
                    // Need to create it.
                    value = valueCreator(textView);
                    properties.Add(key, value);
                    return true;
                }

                // Already there.
                return false;
            }

            AutoClosingViewProperty(TTextView textView) {
                _textView        =  textView;
                _textView.Closed += OnTextViewClosed;
            }

            void OnTextViewClosed(object sender, EventArgs e) {
                _textView.Closed -= OnTextViewClosed;

                if (_textView.Properties.TryGetProperty<AutoClosingViewProperty<TProperty, TTextView>>(typeof(AutoClosingViewProperty<TProperty, TTextView>), out var properties)) {
                    foreach (var disposable in properties.Values.OfType<IDisposable>()) {
                        disposable.Dispose();
                    }
                }

                _textView.Properties.RemoveProperty(typeof(AutoClosingViewProperty<TProperty, TTextView>));
            }

            bool TryGetValue(object key, out TProperty value) {
                return _map.TryGetValue(key, out value);
            }

            void Add(object key, TProperty value) {
                _map[key] = value;
            }

            IEnumerable<TProperty> Values => _map.Values;

        }

    }

}