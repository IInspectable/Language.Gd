using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

namespace Pharmatechnik.Language.Gd {

    abstract class UIElementMetaInfo {

        protected UIElementMetaInfo(IEnumerable<PropertyMetaInfo> properties,
                                    IEnumerable<EventMetaInfo> events,
                                    IEnumerable<SectionMetaInfo> sections) {
            Properties = properties.ToImmutableDictionary(elem => elem.Name);
            Events     = events.ToImmutableDictionary(elem => elem.Name);
            Sections   = sections.ToImmutableDictionary(elem => elem.Name);

        }

        public ImmutableDictionary<string, PropertyMetaInfo> Properties { get; }
        public ImmutableDictionary<string, EventMetaInfo>    Events     { get; }
        public ImmutableDictionary<string, SectionMetaInfo>  Sections   { get; }

        protected static IEnumerable<ValueSuggestion> GetEnumSuggestions<T>() where T : Enum {
            return Enum.GetNames(typeof(T)).Select(n => new ValueSuggestion(n, SuggestionType.EnumValue));
        }

    }

    class LabelMetaInfo: UIElementMetaInfo {

        LabelMetaInfo()
            : base(GetProperties(), GetEvents(), GetSections()) {
        }

        public static readonly LabelMetaInfo Instance = new LabelMetaInfo();

        static IEnumerable<PropertyMetaInfo> GetProperties() {
            yield return new PropertyMetaInfo("Text",  true, typeof(string));
            yield return new PropertyMetaInfo("Key",   true, typeof(string));
            yield return new PropertyMetaInfo("Color", true, typeof(ConsoleColor), GetEnumSuggestions<ConsoleColor>());
        }

        static IEnumerable<EventMetaInfo> GetEvents() {
            yield return new EventMetaInfo("Changed");
        }

        static IEnumerable<SectionMetaInfo> GetSections() {
            yield return new SectionMetaInfo("LAYOUT");
        }

    }

    class GuiDescriptionMetaInfo {

        [CanBeNull]
        public static UIElementMetaInfo FromSection(ISectionSyntax sectionSyntax) {
            ControlSectionSyntax
            return null;
        }

    }
}