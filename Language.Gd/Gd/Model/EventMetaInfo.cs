using System;

namespace Pharmatechnik.Language.Gd {

    public class EventMetaInfo {

        public EventMetaInfo(string name) {
            Name = name ?? throw new ArgumentNullException(nameof(name));

        }

        public string Name { get; }

    }

}