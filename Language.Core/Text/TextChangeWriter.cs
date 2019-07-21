#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Pharmatechnik.Language.Text {

    public class TextChangeWriter {

        public string ApplyTextChanges(string text, IEnumerable<TextChange> textChanges) {

            if (text == null) {
                throw new ArgumentNullException(nameof(text));
            }
            if (textChanges == null) {
                throw new ArgumentNullException(nameof(textChanges));
            }

            var orderedChanges = textChanges.OrderBy(tc => tc.Extent.Start).ToList();
            CheckForOverlappingChanges(orderedChanges);

            StringBuilder textBuilder = new StringBuilder(text);

            int offset = 0;
            foreach (var change in orderedChanges) {
                
                int start = offset + change.Extent.Start;

                textBuilder.Remove(start, change.Extent.Length);
                textBuilder.Insert(start, change.ReplacementText);

                offset += change.ReplacementText.Length - change.Extent.Length;
            }

            return textBuilder.ToString();
        }

        void CheckForOverlappingChanges(IEnumerable<TextChange> orderedChanges) {

            int currentEnd = 0;
            foreach (var change in orderedChanges) {
                if (change.Extent.Start < currentEnd) {
                    throw new ArgumentException("Overlapping changes are not supported");
                }

                currentEnd = change.Extent.End;
            }
        }
    }
}