using System.Collections.Generic;

namespace Pharmatechnik.Language.Gd.Outlining {

    public abstract class OutliningProvider {

        public abstract IEnumerable<OutliningDefinition> GetOutliningDefinitions(SyntaxNode root);

        public static readonly OutliningProvider Default = new AggregationProvider(
            new MultilineCommentOutliningProvider(),
            new SectionSyntaxOutliningProvider());

    }

}