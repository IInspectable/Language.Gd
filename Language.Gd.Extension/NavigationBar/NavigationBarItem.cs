#region

using System.Collections.Immutable;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.NavigationBar {

    class NavigationBarItem {

        public NavigationBarItem(string displayName, int imageIndex): this(displayName, imageIndex, null, -1) {
        }

        public NavigationBarItem(string displayName, int imageIndex, [CanBeNull] Location location, int navigationPoint, ImmutableList<NavigationBarItem> children = null) {
            Extent          = location?.Extent;
            NavigationPoint = navigationPoint;
            DisplayName     = displayName;
            ImageIndex      = imageIndex;
            Children        = children ?? ImmutableList<NavigationBarItem>.Empty;
        }

        /// <summary>
        /// Liefert den Anzeigenamen
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Liefert den Image Index für das item.
        /// </summary>
        public int ImageIndex { get; }

        /// <summary>
        /// Gibt den gesamte Bereich des Items an, oder null, falls es keinen definierten Bereich gibt (z.B. Projekt Items)
        /// </summary>
        [CanBeNull]
        public TextExtent? Extent { get; }

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
        public ImmutableList<NavigationBarItem> Children { get; set; }

    }

}