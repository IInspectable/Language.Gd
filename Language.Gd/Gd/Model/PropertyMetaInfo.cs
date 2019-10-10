using System;
using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd {

    public class PropertyMetaInfo {

        internal PropertyMetaInfo(string name,
                                  bool required,
                                  Type propertyType,
                                  ImmutableArray<ValueSuggestion> suggestions) {
            Name         = name ?? throw new ArgumentNullException(nameof(name));
            Required     = required;
            PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
            Suggestions  = suggestions.IsDefault ? ImmutableArray<ValueSuggestion>.Empty : suggestions;

        }

        public string Name         { get; }
        public bool   Required     { get; }
        public Type   PropertyType { get; }

        public ImmutableArray<ValueSuggestion> Suggestions { get; }

    }

}