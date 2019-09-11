#region Using Directives

using System.Collections.Immutable;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.QuickInfo {

    public sealed class QuickInfoDefinition {

        public QuickInfoDefinition(TextExtent applicableToExtent, Glyph glyph, ImmutableArray<ClassifiedText> content) {
            ApplicableToExtent = applicableToExtent;
            Glyph              = glyph;
            Content            = content.IsDefault ? ImmutableArray<ClassifiedText>.Empty : content;
        }

        public TextExtent                     ApplicableToExtent { get; }
        public Glyph                          Glyph              { get; }
        public ImmutableArray<ClassifiedText> Content            { get; }

    }

}