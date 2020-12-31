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

        public GdOutlineToolWindowSearchOptions() {

            var list = new List<IVsWindowSearchOption> {
                //new WindowSearchBooleanOption(displayText: "Match case",              tooltip: "Match case",              getter: () => MatchCase,             setter: value => MatchCase = value),
                //new WindowSearchBooleanOption(displayText: "Use Regular Expressions", tooltip: "Use Regular Expressions", getter: () => UseRegularExpressions, setter: value => UseRegularExpressions = value)
            };

            SearchOptionsEnum = new WindowSearchOptionEnumerator(list);
        }

        public IVsEnumWindowSearchOptions SearchOptionsEnum { get; }

        //private bool _matchCase;

        //public bool MatchCase {
        //    get => _matchCase;
        //    set {
        //        if (value == _matchCase) {
        //            return;
        //        }

        //        _matchCase = value;

        //        OnPropertyChanged(nameof(MatchCase));
        //    }
        //}

        //private bool _useRegularExpressions;

        //public bool UseRegularExpressions {
        //    get => _useRegularExpressions;
        //    set {
        //        if (value == _useRegularExpressions) {
        //            return;
        //        }

        //        _useRegularExpressions = value;

        //        OnPropertyChanged(nameof(UseRegularExpressions));
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}