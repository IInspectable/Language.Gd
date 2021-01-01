#region Using Directives

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.PatternMatching;

using Pharmatechnik.Language.Gd.Extension.Classification;

using Util = Microsoft.Internal.VisualStudio.PlatformUI.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    public record GdOutlineToolWindowPaneInitParams {

        public IPatternMatcherFactory  PatternMatcherFactory   { get; init; }
        public TextBlockBuilderService TextBlockBuilderService { get; init; }

    }

    [Guid(WindowGuidString)]
    public class GdOutlineToolWindowPane: ToolWindowPane {

        public const string WindowGuidString = "7e927358-0b4d-4953-b2bb-48ef216eb8cb";

        private readonly GdOutlineControl       _outlineControl;
        private readonly GdOutlineController    _outlineController;
        private readonly IPatternMatcherFactory _patternMatcherFactory;

        public GdOutlineToolWindowPane(GdOutlineToolWindowPaneInitParams initParams): base(null) {

            BitmapImageMoniker = KnownMonikers.DocumentOutline;
            ToolBar            = new CommandID(PackageGuids.GdLanguagePackageCmdSetGuid, PackageIds.DocumentOutlineToolWindowToolbar);

            _patternMatcherFactory = initParams.PatternMatcherFactory;

            _outlineControl                                    =  new GdOutlineControl(initParams.TextBlockBuilderService);
            _outlineControl.IsVisibleChanged                   += OnOutlineControlIsVisibleChanged;
            _outlineControl.RequestNavigateToSource            += OnRequestNavigateToSource;
            _outlineControl.RequestNavigateToSecondaryLocation += OnRequestNavigateToSecondaryLocation;

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
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.MaximumMRUItemsProperty.Name,         5u);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchPopupAutoDropdownProperty.Name, false);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchWatermarkProperty.Name,         GetSearchWatermark());
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.ControlMinWidthProperty.Name,         200u);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.ControlMaxWidthProperty.Name,         uint.MaxValue);
            Util.SetValue(pSearchSettings, SearchSettingsDataSource.SearchStartDelayProperty.Name,        500u);

            //var t=Util.QueryValue(pSearchSettings, SearchSettingsDataSource.SearchStartDelayProperty.Name);
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

            _outlineControl.IsVisibleChanged                   -= OnOutlineControlIsVisibleChanged;
            _outlineControl.RequestNavigateToSource            -= OnRequestNavigateToSource;
            _outlineControl.RequestNavigateToSecondaryLocation -= OnRequestNavigateToSecondaryLocation;

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

        void OnRequestNavigateToSecondaryLocation(object sender, RequestNavigateToEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            GdLanguagePackage.GoToLocation(e.OutlineElement.SecondaryNavigationLocation);
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

            var patternMatcher = BuildPatternMatcher(e);

            _outlineControl.ShowOutline(e.OutlineData, patternMatcher);

            UpdateSearchBox();
            UpdateCaption(e.OutlineData);
        }

        [CanBeNull]
        private IPatternMatcher BuildPatternMatcher(OutlineDataEventArgs e) {

            if (String.IsNullOrEmpty(e.SearchString)) {
                return null;
            }

            return _patternMatcherFactory.CreatePatternMatcher(
                e.SearchString,
                new PatternMatcherCreationOptions(CultureInfo.CurrentCulture, PatternMatcherCreationFlags.IncludeMatchedSpans));
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