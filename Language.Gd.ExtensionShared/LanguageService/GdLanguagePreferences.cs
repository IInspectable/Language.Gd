#region Using Directives

using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.LanguageService {

    public class GdLanguagePreferences: LanguagePreferences {

        public GdLanguagePreferences(IServiceProvider site, Guid langSvc, string name)
            : base(site, langSvc, name) {

            EnableCodeSense             = true;
            EnableMatchBraces           = true;
            EnableMatchBracesAtCaret    = true;
            EnableShowMatchingBrace     = true;
            EnableCommenting            = true;
            HighlightMatchingBraceFlags = _HighlightMatchingBraceFlags.HMB_USERECTANGLEBRACES;
            LineNumbers                 = true;
            MaxErrorMessages            = 100;
            AutoOutlining               = true;
            MaxRegionTime               = 2000;
            InsertTabs                  = false;
            IndentSize                  = 4;
            ShowNavigationBar           = true;
            EnableAsyncCompletion       = true;
            WordWrap                    = false;
            WordWrapGlyphs              = true;
            AutoListMembers             = true;
            EnableQuickInfo             = true;
            ParameterInformation        = true;
            HideAdvancedMembers         = false;
        }

        public event EventHandler Changed;

        public override int OnUserPreferencesChanged2(VIEWPREFERENCES2[] viewPrefs, FRAMEPREFERENCES2[] framePrefs, LANGPREFERENCES2[] langPrefs, FONTCOLORPREFERENCES2[] fontColorPrefs) {

            base.OnUserPreferencesChanged2(viewPrefs, framePrefs, langPrefs, fontColorPrefs);

            Changed?.Invoke(this, EventArgs.Empty);

            return VSConstants.S_OK;
        }

    }

}