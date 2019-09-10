using System;

namespace Pharmatechnik.Language.Gd {

    partial class SimplifiedClassificationBuilder {

        IDisposable SupressTokenSeparator() {
            return new SuppressTokenSeparatorGuard(this);
        }

        sealed class SuppressTokenSeparatorGuard: IDisposable {

            private readonly SimplifiedClassificationBuilder _parent;
            private readonly bool                            _savedValue;

            public SuppressTokenSeparatorGuard(SimplifiedClassificationBuilder parent) {
                _parent                        = parent;
                _savedValue                    = parent._suppressTokenSeparator;
                parent._suppressTokenSeparator = true;

            }

            public void Dispose() {
                _parent._suppressTokenSeparator = _savedValue;
            }

        }

        private void AddTokenSeparator() {

            if (!_suppressTokenSeparator && !_emptyLine) {
                AddWhitespace();
            }
        }

        private bool _suppressTokenSeparator;

    }

}