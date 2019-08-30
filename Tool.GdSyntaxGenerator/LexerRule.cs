namespace Tool.GdSyntaxGenerator {

    class LexerRule {

        public string Name            { get; set; }
        public int    Index           { get; set; }
        public bool   IsSimpleTerminal   { get; set; }
        public string TerminalText { get; set; }

    }

}