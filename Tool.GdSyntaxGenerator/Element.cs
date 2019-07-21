namespace Tool.GdSyntaxGenerator {

    class Element {

        public string Name         { get; set; }
        public string Cardinalität { get; set; }

        public override string ToString() {
            return $"{Name}{Cardinalität}";
        }

    }

    class RuleElement: Element {

    }

    class TokenElement: Element {

    }

}