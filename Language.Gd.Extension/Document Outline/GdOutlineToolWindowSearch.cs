#region Using Directives

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class GdOutlineToolWindowSearch: VsSearchTask {

        private readonly GdOutlineController _controller;

        public GdOutlineToolWindowSearch(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback, GdOutlineController controller): base(dwCookie, pSearchQuery, pSearchCallback) {
            _controller = controller;
        }

        protected override void OnStartSearch() {
            ThreadHelper.JoinableTaskFactory.Run(async () => {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _controller.SearchString = SearchQuery.SearchString;

            });

            base.OnStartSearch();
        }

        protected override void OnStopSearch() {
        }

    }

}