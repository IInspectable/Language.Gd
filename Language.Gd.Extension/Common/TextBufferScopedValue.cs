using System;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text;

namespace Pharmatechnik.Language.Gd.Extension.Common {

    sealed class TextBufferScopedValue<T> where T : class {

        T _value;

        TextBufferScopedValue(T value) {
            _value = value;
        }

        internal T Value {
            get { return _value; }
        }

        internal void Dispose() {
            _value = null;
        }

        internal static TextBufferScopedValue<T> GetOrCreate(ITextBuffer textBuffer, object key, Func<T> createFunc) {

            var value = TryGet(textBuffer, key);

            if (value == null) {
                value = createFunc();
                textBuffer.Properties.GetOrCreateSingletonProperty(key, () => new WeakReference(value));
            }

            return new TextBufferScopedValue<T>(value);
        }

        [CanBeNull]
        internal static T TryGet(ITextBuffer textBuffer, object key) {
            T value = null;
            if (textBuffer.Properties.TryGetProperty(key, out WeakReference weakValue)) {
                value = weakValue.Target as T;
                if (value == null) {
                    textBuffer.Properties.RemoveProperty(key);
                }
            }

            return value;
        }

    }

}