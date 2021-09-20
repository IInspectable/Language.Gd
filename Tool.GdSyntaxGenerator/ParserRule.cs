using System;
using System.Collections.Generic;
using System.Linq;

namespace Tool.GdSyntaxGenerator {

    class ParserRule {

        public string            Name         { get; set; }
        public List<Alternative> Alternatives { get; } = new();

        public override string ToString() {
            var alt = String.Join("    |  ", Alternatives.Select(a => a + "\r\n").ToArray());
            return $"{Name}\r\n"   +
                   $"    :  {alt}" +
                   $"    ;";
        }

    }

}