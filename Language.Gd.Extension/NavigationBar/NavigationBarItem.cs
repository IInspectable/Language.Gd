#region

using System.Collections.Immutable;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

using Microsoft.VisualStudio.Imaging.Interop;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.NavigationBar {

    class NavigationBarItem {

        public NavigationBarItem(string displayName, ImageMoniker imageMoniker)
            : this(displayName, imageMoniker, null, -1) {
        }

        public NavigationBarItem(string displayName, ImageMoniker imageMoniker, TextExtent? extent, int navigationPoint, ImmutableList<NavigationBarItem> children = null) {
            Extent          = extent;
            NavigationPoint = navigationPoint;
            DisplayName     = displayName;
            ImageMoniker  = imageMoniker;
            Children        = children ?? ImmutableList<NavigationBarItem>.Empty;
        }

        /// <summary>
        /// Liefert den Anzeigenamen
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Liefert den Moniker für das anzuzeigende Icon
        /// </summary>
        public ImageMoniker ImageMoniker { get; }

        /// <summary>
        /// Gibt den gesamte Bereich des Items an, oder null, falls es keinen definierten Bereich gibt (z.B. Projekt Items)
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
        public ImmutableList<NavigationBarItem> Children { get; set; }

    }

}