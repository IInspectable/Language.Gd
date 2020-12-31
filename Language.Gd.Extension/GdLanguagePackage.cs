#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

using EnvDTE;

using JetBrains.Annotations;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.PatternMatching;
using Microsoft.VisualStudio.TextManager.Interop;

using Pharmatechnik.Language.Gd.Extension.Classification;
using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Gd.Extension.Document_Outline;
using Pharmatechnik.Language.Gd.Extension.LanguageService;
using Pharmatechnik.Language.Gd.Extension.Logging;
using Pharmatechnik.Language.Text;

using Constants = EnvDTE.Constants;
using Project = Microsoft.CodeAnalysis.Project;
using Task = System.Threading.Tasks.Task;

#endregion

namespace Pharmatechnik.Language.Gd.Extension {

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [ProvideLanguageService(typeof(GdLanguageService),
        GdLanguageContentDefinitions.LanguageName,
        101,
        AutoOutlining               = true,
        MatchBraces                 = true,
        ShowSmartIndent             = true,
        DefaultToInsertSpaces       = true,
        MatchBracesAtCaret          = true,
        EnableAsyncCompletion       = true,
        ShowCompletion              = true,
        RequestStockColors          = true,
        EnableLineNumbers           = true,
        EnableAdvancedMembersOption = false,
        ShowMatchingBrace           = true,
        ShowDropDownOptions         = true)]
    [ProvideLanguageExtension(typeof(GdLanguageService), GdLanguageContentDefinitions.FileExtension)]
    [PackageRegistration(UseManagedResourcesOnly                = true, AllowsBackgroundLoading = true)]
    [ProvideService(typeof(GdLanguageService), IsAsyncQueryable = true)]
    [ProvideService(typeof(IServiceProvider), IsAsyncQueryable  = true)]
    [Guid(PackageGuids.GdLanguagePackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(GdOutlineToolWindowPane),
        Style  = VsDockStyle.Tabbed,
        Window = Constants.vsWindowKindSolutionExplorer)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class GdLanguagePackage: AsyncPackage {

        static readonly Logger Logger = Logger.Create<GdLanguagePackage>();

        [CanBeNull] private static GdLanguagePackage _instance;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.

            AddService(typeof(GdLanguageService),
                       (container, ct, type) => Task.FromResult<object>(new GdLanguageService(this)), promote: true);

            AddService(typeof(IServiceProvider),
                       (container, ct, type) => Task.FromResult<object>(this), promote: true);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var commandService = await GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;

            // Leider hilft uns das Laden der IXOS Essentials an dieser Stelle gar nichts, da die Toolbar des Gui Outline Fensters schon
            // vor dem Laden dieses Pakets angezeigt werden kann. Und bereits da würden die Befehle der IXOS Essentials gebraucht...

            //var shell          = await GetServiceAsync(typeof(SVsShell)) as IVsShell;

            //// ReSharper disable once SuspiciousTypeConversion.Global
            //if (shell is IVsShell7 shell7                                                                                                   &&
            //    ErrorHandler.Succeeded(shell.IsPackageInstalled(ref IxosEssentialsCommandIds.GuidIXOSEssentialsPackage, out var installed)) &&
            //    installed != 0) {
            //    await shell7.LoadPackageAsync(IxosEssentialsCommandIds.GuidIXOSEssentialsPackage);
            //}

            OutlineWindowShowCommand.Register(this, commandService);
            OutlineWindowSearchCommand.Register(this, commandService);
            OutlineWindowExpandAllCommand.Register(this, commandService);
            OutlineWindowCollapseAllCommand.Register(this, commandService);
            GdPreviewSelectionCommand.Register(this, commandService);
            GdGenerateSelectionCommand.Register(this, commandService);

            _instance = this;

        }

        [CanBeNull]
        static IServiceProvider GetServiceProvider() {
            return _instance;
        }

        public static object GetGlobalService<TService>() where TService : class {
            return GetGlobalService(typeof(TService));
        }

        public static TInterface GetGlobalService<TService, TInterface>() where TInterface : class {
            return GetGlobalService(typeof(TService)) as TInterface;
        }

        public static _DTE DTE {
            get {
                _DTE dte = GetGlobalService<_DTE, _DTE>();
                return dte;
            }
        }

        public static IServiceProvider ServiceProvider => GetGlobalService<IServiceProvider, IServiceProvider>();

        public static void InvokeCommand(CommandID commandId) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var oleCommandTarget = ServiceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;

            oleCommandTarget?.GlobalInvoke(commandId);

        }

        public static GdLanguageService Language => GetGlobalService<GdLanguageService, GdLanguageService>();

        public static VisualStudioWorkspace Workspace {
            get {
                var componentModel = GetGlobalService<SComponentModel, IComponentModel>();
                var workspace      = componentModel.GetService<VisualStudioWorkspace>();
                return workspace;
            }
        }

        public static Project GetContainingProject(string filePath) {

            Dispatcher.CurrentDispatcher.VerifyAccess();

            var dteSolution = DTE.Solution;
            if (dteSolution == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: There's no DTE solution");
                return null;
            }

            if (string.IsNullOrEmpty(filePath)) {
                Logger.Info($"{nameof(GetContainingProject)}: The text document has not path.");
                return null;
            }

            var projectItem = dteSolution.FindProjectItem(filePath);

            if (projectItem == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: Unable to find a DTE project item with the path '{filePath}'");
                return null;
            }

            var containingProject = projectItem.ContainingProject;
            if (containingProject == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: Project item with the path '{filePath}' has no containing project.");
                return null;
            }

            var projectPath = containingProject.FullName;
            if (string.IsNullOrEmpty(projectPath)) {
                Logger.Info($"{nameof(GetContainingProject)}: Containing project '{containingProject.Name}' for the item with the path '{filePath}' has no full path.");
                return null;
            }

            var roslynSolution = Workspace.CurrentSolution;
            if (roslynSolution == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: No roslyn solution available");
                return null;
            }

            var project = roslynSolution.Projects.FirstOrDefault(p => p.FilePath.ToLower() == projectPath.ToLower());
            if (project == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: Unable to find a roslyn project for the project '{projectPath.ToLower()}'.\nRoslyn Projects:\n{ProjectPaths(roslynSolution.Projects)}");
                return null;
            }

            return project;

            string ProjectPaths(IEnumerable<Project> projects) {
                return projects.Aggregate(new StringBuilder(), (sb, p) => sb.AppendLine(p.FilePath), sb => sb.ToString());
            }
        }

        /// <summary>
        /// 1. Moves the caret to the specified index in the current snapshot.  
        /// 2. Updates the viewport so that the caret will be centered.
        /// 3. Moves focus to the text view to ensure the user can continue typing.
        /// </summary>
        public static void NavigateToLocation(ITextView textView, int location) {

            var bufferPosition = new SnapshotPoint(textView.TextBuffer.CurrentSnapshot, location);

            textView.Caret.MoveTo(bufferPosition);
            textView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(bufferPosition, 1), EnsureSpanVisibleOptions.AlwaysCenter);

            // ReSharper disable once SuspiciousTypeConversion.Global 
            (textView as Control)?.Focus();
        }

        [CanBeNull]
        public static IWpfTextView GoToLocation(Location location, bool openInPreviewTab = false) {

            using (Logger.LogBlock(nameof(GoToLocation))) {

                ThreadHelper.ThrowIfNotOnUIThread();

                if (location == null) {
                    return null;
                }

                IWpfTextView wpfTextView = null;
                if (location.FilePath != null) {
                    if (openInPreviewTab) {
                        wpfTextView = OpenFileInPreviewTab(location.FilePath);
                    } else {
                        wpfTextView = OpenFile(location.FilePath);
                    }
                }

                if (wpfTextView == null) {
                    return null;
                }

                if (location.Start == 0 && location.Length == 0) {
                    return wpfTextView;
                }

                var outliningManagerService = GetServiceProvider().GetMefService<IOutliningManagerService>();

                var snapshotSpan = location.ToSnapshotSpan(wpfTextView.TextSnapshot);
                if (wpfTextView.TryMoveCaretToAndEnsureVisible(snapshotSpan.Start, outliningManagerService)) {
                    wpfTextView.SetSelection(snapshotSpan);
                }

                return wpfTextView;
            }
        }

        [CanBeNull]
        static IWpfTextView GetWpfTextViewFromFrame(IVsWindowFrame frame) {

            ThreadHelper.ThrowIfNotOnUIThread();

            using (Logger.LogBlock(nameof(GetWpfTextViewFromFrame))) {
                if (ErrorHandler.Failed(frame.GetProperty((int) __VSFPROPID.VSFPROPID_DocView, out var docView))) {
                    Logger.Error("Get __VSFPROPID.VSFPROPID_DocView failed");
                    return null;
                }

                if (docView is IVsCodeWindow window) {
                    if (ErrorHandler.Failed(window.GetPrimaryView(out var textView))) {
                        Logger.Error("GetPrimaryView failed");
                        return null;
                    }

                    var model          = (IComponentModel) GetGlobalService(typeof(SComponentModel));
                    var adapterFactory = model.GetService<IVsEditorAdaptersFactoryService>();
                    var wpfTextView    = adapterFactory.GetWpfTextView(textView);
                    return wpfTextView;
                }

                Logger.Warn($"{nameof(GetWpfTextViewFromFrame)}: {nameof(docView)} ist kein {nameof(IVsCodeWindow)}");
                return null;
            }
        }

        [CanBeNull]
        public static IWpfTextView OpenFile(string file) {

            using (Logger.LogBlock(nameof(OpenFile))) {

                ThreadHelper.ThrowIfNotOnUIThread();

                var serviceProvider = GetServiceProvider();

                Guid logicalView = Guid.Empty;
                VsShellUtilities.OpenDocument(serviceProvider, file, logicalView, out var _, out var _, out var windowFrame);

                return GetWpfTextViewFromFrame(windowFrame);
            }
        }

        [CanBeNull]
        public static IWpfTextView OpenFileInPreviewTab(string file) {

            ThreadHelper.ThrowIfNotOnUIThread();

            using (Logger.LogBlock(nameof(OpenFileInPreviewTab))) {

                var state = __VSNEWDOCUMENTSTATE.NDS_Provisional; // | __VSNEWDOCUMENTSTATE.NDS_NoActivate;
                using (new NewDocumentStateScope(state, VSConstants.NewDocumentStateReason.Navigation)) {
                    return OpenFile(file);
                }
            }
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType) {

            if (toolWindowType.Equals(Guid.Parse(GdOutlineToolWindowPane.WindowGuidString))) {
                return this;
            }

            return null;
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id) {

            if (toolWindowType == typeof(GdOutlineToolWindowPane)) {
                return GdOutlineToolWindowPane.DefaultCaption;
            }

            return base.GetToolWindowTitle(toolWindowType, id);

        }

        protected override async Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken) {

            if (toolWindowType == typeof(GdOutlineToolWindowPane)) {
                // Perform as much work as possible in this method which is being run on a background thread.
                // The object returned from this method is passed into the constructor of the SampleToolWindow 
                var cmp = await GetServiceAsync(typeof(SComponentModel)) as IComponentModel;

                var tbbs = cmp?.GetService<TextBlockBuilderService>();
                var pmf  = cmp?.GetService<IPatternMatcherFactory>();

                return new GdOutlineToolWindowPaneInitParams {
                    TextBlockBuilderService = tbbs, 
                    PatternMatcherFactory   = pmf
                };
            }

            return null;
        }

    }

}