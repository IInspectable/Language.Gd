#region Using Directives

using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Language.Gd.Outlining {

    sealed class AggregationProvider: OutliningProvider {

        readonly ImmutableArray<OutliningProvider> _outliningProviders;

        public AggregationProvider(params OutliningProvider[] providers) {
            _outliningProviders = providers.ToImmutableArray();
        }

        public override IEnumerable<OutliningDefinition> GetOutliningDefinitions(SyntaxNode root) {

            return _outliningProviders.SelectMany(provider => provider.GetOutliningDefinitions(root));

        }

    }

}