#region Using Directives

using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Gd.Extension.Logging;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.LanguageService {

    class GdCodeWindowManager: IVsCodeWindowManager {

        static readonly Logger Logger = Logger.Create<GdCodeWindowManager>();

        readonly IVsCodeWindow    _codeWindow;
        readonly IServiceProvider _serviceProvider;

        NavigationBar.NavigationBar _navigationBar;

        public GdCodeWindowManager(GdLanguageService languageService, IServiceProvider serviceProvider, IVsCodeWindow codeWindow) {
            LanguageService  = languageService;
            _codeWindow      = codeWindow;
            _serviceProvider = serviceProvider;
        }

        public GdLanguageService LanguageService { get; }

        public int OnNewView(IVsTextView pView) {
            return VSConstants.S_OK;
        }

        public int AddAdornments() {

            AddOrRemoveDropdown(showNavigationBar: LanguageService.Preferences.ShowNavigationBar);

            LanguageService.Preferences.Changed += OnPreferencesChanged;

            return VSConstants.S_OK;
        }

        public int RemoveAdornments() {

            AddOrRemoveDropdown(showNavigationBar: false);

            LanguageService.Preferences.Changed -= OnPreferencesChanged;

            return VSConstants.S_OK;
        }

        private void OnPreferencesChanged(object sender, EventArgs e) {
            AddOrRemoveDropdown(LanguageService.Preferences.ShowNavigationBar);
        }

        void AddOrRemoveDropdown(bool showNavigationBar) {

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(_codeWindow is IVsDropdownBarManager dropdownManager)) {
                return;
            }

            if (showNavigationBar) {
                var existingDropdownBar = GetDropdownBar(dropdownManager);
                if (existingDropdownBar != null) {

                    // Check if the existing dropdown is already one of ours, and do nothing if it is.
                    if (_navigationBar != null &&
                        _navigationBar == GetDropdownBarClient(existingDropdownBar)) {
                        return;
                    }

                    // Not ours, so remove the old one so that we can add ours.
                    RemoveDropdownBar(dropdownManager);
                }

                AddDropdownBar(dropdownManager);
            } else {
                RemoveDropdownBar(dropdownManager);
            }
        }

        void AddDropdownBar(IVsDropdownBarManager dropdownManager) {

            _codeWindow.GetPrimaryView(out var textView);

            if (textView == null) {
                Logger.Warn($"{nameof(AddDropdownBar)}: Unable to get primary view");
                return;
            }

            var editorAdaptersFactoryService = _serviceProvider.GetMefService<IVsEditorAdaptersFactoryService>();

            var wpfTextView = editorAdaptersFactoryService.GetWpfTextView(textView);
            if (wpfTextView == null) {
                Logger.Warn($"{nameof(AddDropdownBar)}: Unable to get IWpfTextView");
                return;
            }

            var dropdownBarClient = new NavigationBar.NavigationBar(wpfTextView.TextBuffer, dropdownManager, _codeWindow, _serviceProvider);

            #if ShowMemberCombobox
            var hr = dropdownManager.AddDropdownBar(cCombos: 3, pClient: dropdownBarClient);
            #else
            var hr = dropdownManager.AddDropdownBar(cCombos: 2, pClient: dropdownBarClient);
            #endif
            if (ErrorHandler.Failed(hr)) {
                ErrorHandler.ThrowOnFailure(hr);
            }

            _navigationBar = dropdownBarClient;
        }

        void RemoveDropdownBar(IVsDropdownBarManager dropdownManager) {
            dropdownManager.RemoveDropdownBar();

            _navigationBar?.Dispose();
            _navigationBar = null;
        }

        static IVsDropdownBar GetDropdownBar(IVsDropdownBarManager dropdownManager) {
            ErrorHandler.ThrowOnFailure(dropdownManager.GetDropdownBar(out var existingDropdownBar));
            return existingDropdownBar;
        }

        static IVsDropdownBarClient GetDropdownBarClient(IVsDropdownBar dropdownBar) {
            ErrorHandler.ThrowOnFailure(dropdownBar.GetClient(out var dropdownBarClient));
            return dropdownBarClient;
        }

    }

}