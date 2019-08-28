#region Using Directives

using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.LanguageService {

    [Guid(GdLanguagePackage.Guids.LanguageGuidString)]
    public class GdLanguageService: IVsLanguageInfo {

        readonly GdLanguagePackage _package;

        private GdLanguagePreferences _preferences;

        public GdLanguageService(GdLanguagePackage package) {
            _package = package;
        }

        public GdLanguagePreferences Preferences {
            get {
                if (_preferences == null) {
                    _preferences = new GdLanguagePreferences(_package, typeof(GdLanguageService).GUID, GdLanguageContentDefinitions.LanguageName);
                    _preferences.Init();
                }

                return _preferences;
            }
        }

        public int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr) {

            ppCodeWinMgr = new GdCodeWindowManager(this, _package, pCodeWin);

            return VSConstants.S_OK;
        }

        public int GetColorizer(IVsTextLines pBuffer, out IVsColorizer ppColorizer) {
            ppColorizer = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetFileExtensions(out string pbstrExtensions) {
            pbstrExtensions = GdLanguageContentDefinitions.FileExtension;
            return VSConstants.S_OK;
        }

        public int GetLanguageName(out string bstrName) {
            bstrName = GdLanguageContentDefinitions.LanguageName;
            return VSConstants.S_OK;
        }

    }

}