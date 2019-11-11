using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.DocumentOutline {

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(), nq}")]
    public class OutlineElement {

        public OutlineElement(ImmutableArray<ClassifiedText> displayParts,
                              TextExtent extent,
                              int navigationPoint,
                              Glyph glyph,
                              ImmutableArray<OutlineElement> children = default,
                              Location secondaryNavigationLocation = default) {

            DisplayParts                = displayParts;
            Extent                      = extent;
            NavigationPoint             = navigationPoint;
            SecondaryNavigationLocation = secondaryNavigationLocation;
            Glyph                       = glyph;
            Children                    = children.IsDefault ? ImmutableArray<OutlineElement>.Empty : children;

        }

        public ImmutableArray<ClassifiedText> DisplayParts { get; }

        /// <summary>
        /// Gibt den gesamten Bereich des Items an, oder null, falls es keinen definierten Bereich gibt (z.B. Projekt Items)
        /// </summary>
        public TextExtent Extent { get; }

        /// <summary>
        /// Liefert den Anzeigenamen
        /// </summary>
        public string DisplayName => DisplayParts.JoinText();

        /// <summary>
        /// Liefert den Moniker für das anzuzeigende Icon
        /// </summary>
        public Glyph Glyph { get; }

        /// <summary>
        /// Gibt die Stelle an, an die bei Auswahl des Items hinnavigiert werden soll.
        /// </summary>
        public int NavigationPoint { get; }

        /// <summary>
        /// Gibt den sekundären Navigationspunkt an, falls vorhanden.
        /// </summary>
        [CanBeNull]
        public Location SecondaryNavigationLocation { get; }

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

                // Wir gehen davon aus, dass die Elemente hierarchisch sind, und von daher der Parent immer mindestens die Ausdehung seiner Kinder hat.
                // Um die Ordnung nun beizubehalten, kommen bei Elementen mit selber Position die Elemente mit der größeren Ausdehnung
                // (also die Eltern) immer zuerst.
                return elements.OrderBy(e => e.Extent.Start)
                               .ThenByDescending(e => e.Extent.Length)
                               .ToImmutableArray();
            }

            void FlattenElements(OutlineElement element, ICollection<OutlineElement> items) {
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

            return FindExtactMatch(position) ?? this;
        }

        OutlineElement FindExtactMatch(int position) {
            if (!Extent.Contains(position)) {
                return null;
            }

            if (Children.Length == 0) {
                return this;
            }

            var bestChild = Children.Select(entry => entry.FindExtactMatch(position))
                                    .FirstOrDefault(c => c != null);

            return bestChild ?? this;
        }

    }

}