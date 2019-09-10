#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Language.Text {

    public sealed class TextEditorSettings {

        public TextEditorSettings(int tabSize, string newLine) {
            if (tabSize < 0) {
                throw new ArgumentOutOfRangeException();
            }

            TabSize = tabSize;
            NewLine = newLine ?? throw new ArgumentNullException(nameof(newLine));
        }

        public int    TabSize { get; }
        public string NewLine { get; }

        public static TextEditorSettings Default = new TextEditorSettings(4, "\r\n");

        public TextEditorSettings With(int? tabSize = null,
                                       string newLine = null) {
            return new TextEditorSettings(
                tabSize: tabSize ?? TabSize,
                newLine: newLine ?? NewLine);
        }

    }

}