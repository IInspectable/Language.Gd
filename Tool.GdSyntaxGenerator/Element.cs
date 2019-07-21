namespace Tool.GdSyntaxGenerator {

    class Element {

        public string Name         { get; set; }
        public string Cardinality { get; set; }

        public override string ToString() {
            return $"{Name}{Cardinality}";
        }

    }

    class RuleElement: Element {

    }

    class TokenElement: Element {

    }

}