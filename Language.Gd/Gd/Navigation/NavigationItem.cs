#region Using Directives

using System.Collections.Immutable;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Navigation {

    public class NavigationItem {

        public NavigationItem(string displayName, Glyph glyph)
            : this(displayName, glyph, null, -1) {
        }

        public NavigationItem(string displayName, Glyph glyph, TextExtent? extent, int navigationPoint, ImmutableList<NavigationItem> children = null) {
            Extent          = extent;
            NavigationPoint = navigationPoint;
            DisplayName     = displayName;
            Glyph           = glyph;
            Children        = children ?? ImmutableList<NavigationItem>.Empty;
        }

        /// <summary>
        /// Liefert den Anzeigenamen
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Liefert den Moniker für das anzuzeigende Icon
        /// </summary>
        public Glyph Glyph { get; }

        /// <summary>
        /// Gibt den gesamten Bereich des Items an, oder null, falls es keinen definierten Bereich gibt (z.B. Projekt Items)
        /// </summary>
        [CanBeNull]
        public TextExtent? Extent { get; }

        /// <summary>
        /// Gibt an, ob der angegebene Punkt innerhalb des Bereichs des Items liegt.
        /// Liefert false, falls es keinen definierten Bereich gibt (z.B. Projekt Items)
        /// </summary>
        public bool Contains(int position) {
            return Extent?.Contains(position) ?? false;
        }

        /// <summary>
        /// Gibt den Startpunkt des Bereichs an.
        /// </summary>
        public int Start => Extent?.Start ?? -1;

        /// <summary>
        /// Gibt den Endpunkt des Bereichs an.
        /// </summary>
        public int End => Extent?.End ?? -1;

        /// <summary>
        /// Gibt die Stelle an, an die bei Auswahl des Items hinnavigiert werden soll.
        /// </summary>
        public int NavigationPoint { get; }

        [NotNull]
        public ImmutableList<NavigationItem> Children { get; set; }

    }

}