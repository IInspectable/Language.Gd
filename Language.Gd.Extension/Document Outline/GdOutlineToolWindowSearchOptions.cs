#region Using Directives

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class GdOutlineToolWindowSearchOptions: INotifyPropertyChanged {

        private readonly WindowSearchBooleanOption  _matchCaseOption;
        private          IVsEnumWindowSearchOptions _optionsEnum;

        public GdOutlineToolWindowSearchOptions() {
            _matchCaseOption = new WindowSearchBooleanOption("Match case", "Match case", getter: () => MatchCase, setter: value => MatchCase = value);
        }

        private bool _matchCase;

        public bool MatchCase {
            get => _matchCase;
            set {
                if (value == _matchCase) {
                    return;
                }

                _matchCase = value;

                OnPropertyChanged(nameof(MatchCase));
            }
        }

        public IVsEnumWindowSearchOptions SearchOptionsEnum {
            get {
                if (_optionsEnum == null) {

                    var list = new List<IVsWindowSearchOption> {
                        _matchCaseOption
                    };

                    _optionsEnum = new WindowSearchOptionEnumerator(list);
                }

                return _optionsEnum;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}