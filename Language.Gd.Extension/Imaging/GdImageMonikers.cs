using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Pharmatechnik.Language.Gd.Extension.Imaging {

    public static class GdImageMonikers {

        static readonly ImmutableDictionary<Glyph, ImageMoniker> Mapping = new Dictionary<Glyph, ImageMoniker> {
            {Glyph.None, default},

            {Glyph.Namespace, KnownMonikers.Namespace},

            {Glyph.Dialog, KnownMonikers.Dialog},
            {Glyph.Form, KnownMonikers.Dialog},
            {Glyph.UserControl, KnownMonikers.UserControl},
            {Glyph.BarManager, KnownMonikers.ApplicationBar},
            {Glyph.DetailsPanel, KnownMonikers.DetailView},
            {Glyph.MultiView, KnownMonikers.MultiView},
            {Glyph.Panel, KnownMonikers.Panel},
            {Glyph.TabNavigation, KnownMonikers.Tab},
            {Glyph.TabPage, KnownMonikers.Tab},
            {Glyph.UnknownControl, KnownMonikers.Control},
            {Glyph.AmountTextbox, KnownMonikers.Currency},
            {Glyph.BrowsableTextbox, KnownMonikers.FilteredTextBox},
            {Glyph.Button, KnownMonikers.Button},
            {Glyph.DynamicButton, KnownMonikers.Button},
            {Glyph.Cave, KnownMonikers.InteractionUse},
            {Glyph.Chart, KnownMonikers.ColumnChart},
            {Glyph.Checkbox, KnownMonikers.CheckBoxChecked},
            {Glyph.Combobox, KnownMonikers.ComboBox},
            {Glyph.DateTextbox, KnownMonikers.DateTimePicker},
            {Glyph.Dropdownbox, KnownMonikers.ComboBox},
            {Glyph.DynamicFunctionButton, KnownMonikers.Button},
            {Glyph.DynamicLabel, KnownMonikers.Label},
            {Glyph.ExplorerBar, KnownMonikers.ToolstripPanelLeft},
            {Glyph.FormattedLabel, KnownMonikers.TextBlock},
            {Glyph.FormattedTextbox, KnownMonikers.RichTextBox},
            {Glyph.FunctionButton, KnownMonikers.Button},
            {Glyph.HeaderScroller, KnownMonikers.VerticalScrollBar},
            {Glyph.Label, KnownMonikers.Label},
            {Glyph.MaskTextbox, KnownMonikers.MaskedTextBox},
            {Glyph.NumericTextbox, KnownMonikers.PhoneNumberEditor},
            {Glyph.Picturebox, KnownMonikers.PictureAndText},
            {Glyph.PersistentPictureBox, KnownMonikers.PictureAndText},
            {Glyph.PhoneTextbox, KnownMonikers.PhoneNumberViewer},
            {Glyph.Radiobutton, KnownMonikers.RadioButton},
            {Glyph.ReportPreview, KnownMonikers.Report},
            {Glyph.Scanner, KnownMonikers.InfraredDevice},
            {Glyph.Table, KnownMonikers.Table},
            {Glyph.Column, KnownMonikers.Column},
            {Glyph.ContextMenu, KnownMonikers.ContextMenu},
            {Glyph.Textbox, KnownMonikers.TextBox},
            {Glyph.TimeTextbox, KnownMonikers.TimePicker},
            {Glyph.Tree, KnownMonikers.TreeView},
            {Glyph.UserControlReference, KnownMonikers.InheritedUserControl},
            {Glyph.WebBrowser, KnownMonikers.WebBrowserItem},
            {Glyph.Property, KnownMonikers.Property},
            {Glyph.Event, KnownMonikers.EventPublic},
            

        }.ToImmutableDictionary();

        public static ImageMoniker GetMoniker(Glyph glyph) {
            return Mapping.TryGetValue(glyph, out var imageMoniker) ? imageMoniker : default;
        }

        public static bool HasImageMoniker(Glyph glyph) {
            return Mapping.ContainsKey(glyph);
        }

    }

}