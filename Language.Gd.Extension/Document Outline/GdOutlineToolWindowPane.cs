#region Using Directives

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Pharmatechnik.Language.Gd.Extension.Classification;

using Util = Microsoft.Internal.VisualStudio.PlatformUI.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    [Guid(WindowGuidString)]
    public class GdOutlineToolWindowPane: ToolWindowPane {

        public const string WindowGuidString = "7e927358-0b4d-4953-b2bb-48ef216eb8cb";

        private readonly GdOutlineControl                 _outlineControl;
        private readonly GdOutlineController              _outlineController;
        private readonly GdOutlineToolWindowSearchOptions _searchOptions;

        public GdOutlineToolWindowPane(TextBlockBuilderService textBlockBuilderService): base(null) {

            BitmapImageMoniker = KnownMonikers.DocumentOutline;

            _outlineControl                         =  new GdOutlineControl(textBlockBuilderService);
            _outlineControl.IsVisibleChanged        += OnOutlineControlIsVisibleChanged;
            _outlineControl.RequestNavigateToSource += OnRequestNavigateToSource;

            _outlineController                          =  new GdOutlineController();
            _outlineController.RequestNavigateToOutline += OnRequestNavigateToOutline;
            _outlineController.OutlineDataChanged       += OnOutlineDataChanged;

            _searchOptions                 =  new GdOutlineToolWindowSearchOptions();
            _searchOptions.PropertyChanged += OnSearchOptionsChanged;

            ToolBar = new CommandID(PackageGuids.GdLanguagePackageCmdSetGuid, PackageIds.DocumentOutlineToolWindowToolbar);

            UpdateCaption();

            Instance = this;

        }

        private void OnSearchOptionsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            _outlineController.OnSearchOptionsChanged();
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

        public override IVsEnumWindowSearchOptions SearchOptionsEnum => _searchOptions.SearchOptionsEnum;

        public override void ProvideSearchSettings(IVsUIDataSource pSearchSettings) {
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchStartTypeProperty.Name,         (uint) VSSEARCHSTARTTYPE.SST_DELAYED);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchProgressTypeProperty.Name,      (uint) VSSEARCHPROGRESSTYPE.SPT_NONE);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchUseMRUProperty.Name,            true);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchPopupAutoDropdownProperty.Name, false);
            //Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchWatermarkProperty.Name, GetWatermark());
        }

        public override void ClearSearch() {
            ThreadHelper.ThrowIfNotOnUIThread();
            _outlineController.SearchString = null;
            base.ClearSearch();
        }

        [CanBeNull]
        public static GdOutlineToolWindowPane Instance { get; private set; }

        protected override void Dispose(bool disposing) {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.Dispose(disposing);

            _outlineControl.IsVisibleChanged            -= OnOutlineControlIsVisibleChanged;
            _outlineControl.RequestNavigateToSource     -= OnRequestNavigateToSource;
            _outlineController.RequestNavigateToOutline -= OnRequestNavigateToOutline;
            _outlineController.OutlineDataChanged       -= OnOutlineDataChanged;

            _outlineController.Dispose();
        }

        public override object Content {
            get => _outlineControl;
            set { }
        }

        public bool CanExpandCollapse => _outlineControl.HasItems;

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

            _outlineControl.ShowOutline(e.OutlineData, e.SearchString, _searchOptions);

            UpdateSearchBox();
            UpdateCaption(e.OutlineData);
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

    }

}