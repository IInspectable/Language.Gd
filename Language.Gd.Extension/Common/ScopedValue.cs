using System;

namespace Pharmatechnik.Language.Gd.Extension.Common {

    class ScopedValue<T>: IDisposable {

        readonly T         _savedValue;
        readonly Action<T> _setter;

        public ScopedValue(Func<T> getter, Action<T> setter, T value) {
            _savedValue = getter();
            _setter     = setter;
            _setter(value);
        }

        public void Dispose() {
            _setter(_savedValue);
        }

    }

}