using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.DocumentOutline {

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(), nq}")]
    public class OutlineElement {

        public OutlineElement(string displayName,
                              TextExtent extent,
                              int navigationPoint,
                              Glyph glyph,
                              ImmutableArray<OutlineElement> children = default) {

            DisplayName     = displayName;
            Extent          = extent;
            NavigationPoint = navigationPoint;
            Glyph           = glyph;
            Children        = children.IsDefault ? ImmutableArray<OutlineElement>.Empty : children;

        }

        /// <summary>
        /// Gibt den gesamten Bereich des Items an, oder null, falls es keinen definierten Bereich gibt (z.B. Projekt Items)
        /// </summary>
        public TextExtent Extent { get; }

        /// <summary>
        /// Liefert den Anzeigenamen
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Liefert den Moniker für das anzuzeigende Icon
        /// </summary>
        public Glyph Glyph { get; }

        /// <summary>
        /// Gibt die Stelle an, an die bei Auswahl des Items hinnavigiert werden soll.
        /// </summary>
        public int NavigationPoint { get; }

        public ImmutableArray<OutlineElement> Children { get; }

        /// <summary>
        /// Gibt an, ob der angegebene Punkt innerhalb des Bereichs des Items liegt.
        /// Liefert false, falls es keinen definierten Bereich gibt (z.B. Projekt Items)
        /// </summary>
        public bool Contains(int position) {
            return Extent.Contains(position);
        }

        /// <summary>
        /// Gibt den Startpunkt des Bereichs an.
        /// </summary>
        public int Start => Extent.Start;

        /// <summary>
        /// Gibt die Länge des Bereichs an.
        /// </summary>
        public int Length => Extent.Length;

        /// <summary>
        /// Gibt den Endpunkt des Bereichs an.
        /// </summary>
        public int End => Extent.End;

        ImmutableArray<OutlineElement> _flattenElements;

        public ImmutableArray<OutlineElement> Flatten() {

            if (_flattenElements.IsDefault) {
                lock (this) {
                    if (_flattenElements.IsDefault) {
                        _flattenElements = FlattenImpl();
                    }
                }

            }

            return _flattenElements;

            ImmutableArray<OutlineElement> FlattenImpl() {

                var elements = new List<OutlineElement>();

                FlattenElements(this, elements);

                return elements.OrderBy(e => e.Extent.Start)
                               .ThenBy(e => e.Extent.Length)
                               .ToImmutableArray();
            }

            void FlattenElements(OutlineElement element, List<OutlineElement> items) {
                items.Add(element);
                foreach (var c in element.Children) {
                    FlattenElements(c, items);
                }
            }
        }

        private string GetDebuggerDisplay() {
            return DisplayName;
        }

        [CanBeNull]
        public OutlineElement FindBestMatch(int position) {

            var flattenList = Flatten();
            // finde erstes Item mit dem kleinsten definierten Bereich
            var activeItem = flattenList.Where(entry => entry.Contains(position))
                                        .OrderBy(e => e.Extent.Length)
                                        .FirstOrDefault();

            if (activeItem != null) {
                return activeItem;
            }

            // Den ersten Eintrag nach dem Cursor wählen
            var closestEntry = flattenList.FirstOrDefault(entry => position < entry.Start && position < entry.End);
            if (closestEntry == null) {
                // Den letzten Eintrag wählen
                closestEntry = flattenList.Last();
            }

            return closestEntry;
        }

    }

}