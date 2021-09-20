using System;
using System.Collections.Generic;
using System.Linq;

namespace Tool.GdSyntaxGenerator {

    class Alternative {

        public List<Element> Elements { get; } = new();

        public override string ToString() {
            return String.Join(" ", Elements.Select(r => r.ToString()).ToArray());
        }

    }

}