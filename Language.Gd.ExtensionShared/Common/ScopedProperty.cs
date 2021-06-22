using System;

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class ScopedProperty {

        public static ScopedProperty<bool> Boolean(bool defaultValue = false) => new ScopedProperty<bool>(defaultValue, !defaultValue);

    }

    sealed class ScopedProperty<T> {

        readonly T _scopeValue;
        T          _value;

        public ScopedProperty(T defaultValue, T scopeValue) {
            _value      = defaultValue;
            _scopeValue = scopeValue;
        }

        public T Value => _value;

        public static implicit operator T(ScopedProperty<T> p) => p.Value;

        public IDisposable Enter() {
            return new ScopedValue<T>(
                getter: () => _value,
                setter: v => _value = v,
                scopeValue: _scopeValue);
        }

    }

}