#region Using Directives

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Pharmatechnik.Language.Gd.Extension.Classification;
using Pharmatechnik.Language.Gd.Extension.Common;

using Util = Microsoft.Internal.VisualStudio.PlatformUI.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    [Guid(WindowGuidString)]
    public class GdOutlineToolWindowPane: ToolWindowPane {

        public const string WindowGuidString = "7e927358-0b4d-4953-b2bb-48ef216eb8cb";

        private readonly GdOutlineControl    _outlineControl;
        private readonly GdOutlineController _outlineController;

        public GdOutlineToolWindowPane(TextBlockBuilderService textBlockBuilderService): base(null) {

            BitmapImageMoniker = KnownMonikers.DocumentOutline;
            ToolBar            = new CommandID(PackageGuids.GdLanguagePackageCmdSetGuid, PackageIds.DocumentOutlineToolWindowToolbar);

            _outlineControl                         =  new GdOutlineControl(textBlockBuilderService);
            _outlineControl.IsVisibleChanged        += OnOutlineControlIsVisibleChanged;
            _outlineControl.RequestNavigateToSource += OnRequestNavigateToSource;

            _outlineController                          =  new GdOutlineController();
            _outlineController.RequestNavigateToOutline += OnRequestNavigateToOutline;
            _outlineController.OutlineDataChanged       += OnOutlineDataChanged;

            UpdateCaption();

            Instance = this;

        }

        public override bool SearchEnabled => true;

        public override IVsSearchTask CreateSearch(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback) {

            if (pSearchQuery == null || pSearchCallback == null)
                return null;

            return new GdOutlineToolWindowSearch(dwCookie,
                                                 pSearchQuery,
                                                 pSearchCallback,
                                                 _outlineController
            );
        }

        public override IVsEnumWindowSearchOptions SearchOptionsEnum => _outlineController.SearchOptions.SearchOptionsEnum;

        public override void ProvideSearchSettings(IVsUIDataSource pSearchSettings) {
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchStartTypeProperty.Name,         (uint) VSSEARCHSTARTTYPE.SST_DELAYED);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchProgressTypeProperty.Name,      (uint) VSSEARCHPROGRESSTYPE.SPT_NONE);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchUseMRUProperty.Name,            true);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.MaximumMRUItemsProperty.Name,         (uint) 5);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchPopupAutoDropdownProperty.Name, false);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchWatermarkProperty.Name,         GetSearchWatermark());
        }

        private string GetSearchWatermark() {

            var watermark = "Search Outline";

            var keyBinding = KeyBindingHelper.GetGlobalKeyBinding(PackageGuids.GdLanguagePackageCmdSetGuid, PackageIds.OutlineWindowSearchId);
            if (!String.IsNullOrEmpty(keyBinding)) {
                watermark += $" ({keyBinding})";
            }

            return watermark;

        }

        public override void ClearSearch() {
            ThreadHelper.ThrowIfNotOnUIThread();
            _outlineController.SearchString = null;
            base.ClearSearch();
        }

        public override bool OnNavigationKeyDown(uint dwNavigationKey, uint dwModifiers) {

            var modifier      = (__VSUIACCELMODIFIERS) dwModifiers;
            var navigationKey = (__VSSEARCHNAVIGATIONKEY) dwNavigationKey;

            if (modifier != __VSUIACCELMODIFIERS.VSAM_NONE) {
                return false;
            }

            switch (navigationKey) {
                case __VSSEARCHNAVIGATIONKEY.SNK_DOWN:
                    return _outlineControl.NavigateDown();
                case __VSSEARCHNAVIGATIONKEY.SNK_UP:
                    return _outlineControl.NavigateUp();
            }

            return false;
        }

        [CanBeNull]
        public static GdOutlineToolWindowPane Instance { get; private set; }

        protected override void Dispose(bool disposing) {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.Dispose(disposing);

            _outlineControl.IsVisibleChanged        -= OnOutlineControlIsVisibleChanged;
            _outlineControl.RequestNavigateToSource -= OnRequestNavigateToSource;

            _outlineController.RequestNavigateToOutline -= OnRequestNavigateToOutline;
            _outlineController.OutlineDataChanged       -= OnOutlineDataChanged;

            _outlineController.Dispose();
        }

        public override object Content {
            get => _outlineControl;
            set { }
        }

        public bool HasContent => _outlineControl.HasItems;

        public void CollapseAll() {
            _outlineControl.CollapseAll();
        }

        public void ExpandAll() {
            _outlineControl.ExpandAll();
        }

        void OnRequestNavigateToSource(object sender, RequestNavigateToEventArgs e) {
            _outlineController.NavigateToSource(e.OutlineElement);
        }

        void OnRequestNavigateToOutline(object sender, NavigateToOutlineEventArgs e) {
            _outlineControl.NavigateToOutline(e.OutlineElement);
        }

        void OnOutlineControlIsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (_outlineControl.IsVisible) {
                _outlineController.Run();
            } else {
                _outlineController.Stop();
            }
        }

        public override void OnToolWindowCreated() {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.OnToolWindowCreated();
            _outlineController.Run();
            UpdateSearchBox();
        }

        private void OnOutlineDataChanged(object sender, OutlineDataEventArgs e) {
            
            ThreadHelper.ThrowIfNotOnUIThread();

            var searchPattern = BuildSearchPattern(e);

            _outlineControl.ShowOutline(e.OutlineData, searchPattern);

            UpdateSearchBox();
            UpdateCaption(e.OutlineData);
        }

        private static Regex BuildSearchPattern(OutlineDataEventArgs e) {

            // TODO Hier könnte man evtl. Syntaxfehler abfangen und als Infobar anzeigen
            var searchPattern = RegexUtil.BuildSearchPattern(searchString: e.SearchString,
                                                             matchCase: e.SearchOptions.MatchCase,
                                                             useRegularExpressions: e.SearchOptions.UseRegularExpressions);
            return searchPattern;
        }

        public const string DefaultCaption = "Gui Outline";

        void UpdateSearchBox() {
            ThreadHelper.ThrowIfNotOnUIThread();

            SearchHost.IsEnabled = _outlineController.HasActiveTextView;
        }

        private void UpdateCaption([CanBeNull] OutlineData outlineData = null) {

            var file = outlineData?.SyntaxTree.SourceText.FileInfo?.Name;
            if (!String.IsNullOrEmpty(file)) {
                Caption = $"{DefaultCaption} - {file}";
            } else {
                Caption = DefaultCaption;
            }
        }

        public void ActivateSearch() {
            ThreadHelper.ThrowIfNotOnUIThread();
            SearchHost?.Activate();
        }

    }

}