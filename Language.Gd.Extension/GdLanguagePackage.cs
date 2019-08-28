#region Using Directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.LanguageServices;

using Pharmatechnik.Language.Gd.Extension.LanguageService;
using Pharmatechnik.Language.Gd.Extension.Logging;

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
        ShowSmartIndent             = false,
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
    [Guid(Guids.PackageGuidString)]
    public sealed class GdLanguagePackage: AsyncPackage {

        static readonly Logger Logger = Logger.Create<GdLanguagePackage>();

        public static class Guids {

            public const string LanguageGuidString = "6B03358E-B39A-4496-A389-764B41F50EAF";
            public const string PackageGuidString  = "ECAD3E41-42EA-49C5-BE56-4865B09C0FCD";

        }

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
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            AddService(typeof(GdLanguageService), async (container, ct, type)
                           => {
                           await JoinableTaskFactory.SwitchToMainThreadAsync(ct);
                           return new GdLanguageService(this);
                       }, promote: true);
        }

        public static object GetGlobalService<TService>() where TService : class {
            return GetGlobalService(typeof(TService));
        }

        public static TInterface GetGlobalService<TService, TInterface>() where TInterface : class {
            return GetGlobalService(typeof(TService)) as TInterface;
        }

        // ReSharper disable once UnusedMember.Local
        static IServiceProvider GetServiceProvider() {
            var serviceProvider = GetGlobalService<GdLanguagePackage, IServiceProvider>();
            return serviceProvider;
        }

        public static _DTE DTE {
            get {
                _DTE dte = GetGlobalService<_DTE, _DTE>();
                return dte;
            }
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

        public static IntPtr GetImageList(ImageMoniker moniker, Color? backgroundColor = null) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var imageAttributes = GetImageAttributes(_UIImageType.IT_ImageList, _UIDataFormat.DF_Win32, backgroundColor);
            var imageService    = GetGlobalService<SVsImageService, IVsImageService2>();
            var result          = imageService?.GetImage(moniker, imageAttributes);

            if (!(Microsoft.Internal.VisualStudio.PlatformUI.Utilities.GetObjectData(result) is IVsUIWin32ImageList imageListData)) {
                Logger.Warn($"{nameof(GetImageList)}: Unable to get IVsUIWin32ImageList");
                return IntPtr.Zero;
            }

            if (!ErrorHandler.Succeeded(imageListData.GetHIMAGELIST(out var imageListInt))) {
                Logger.Warn($"{nameof(GetImageList)}: Unable to get HIMAGELIST");
                return IntPtr.Zero;

            }

            return (IntPtr) imageListInt;
        }

        static ImageAttributes GetImageAttributes(_UIImageType imageType, _UIDataFormat format, Color? backgroundColor, int width = 16, int height = 16) {

            ImageAttributes imageAttributes = new ImageAttributes {
                StructSize    = Marshal.SizeOf(typeof(ImageAttributes)),
                Dpi           = 96,
                Flags         = (uint) _ImageAttributesFlags.IAF_RequiredFlags,
                ImageType     = (uint) imageType,
                Format        = (uint) format,
                LogicalHeight = height,
                LogicalWidth  = width
            };
            if (backgroundColor.HasValue) {
                unchecked {
                    imageAttributes.Flags |= (uint) _ImageAttributesFlags.IAF_Background;
                }

                imageAttributes.Background = (uint) backgroundColor.Value.ToArgb();
            }

            return imageAttributes;
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

    }

}