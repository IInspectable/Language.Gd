namespace Tool.GdSyntaxGenerator.Models {

    class TokenModel {

        public TokenModel(TokenInfo tokenInfo, string baseNamespace) {
            TokenInfo = tokenInfo;
            Namespace = baseNamespace;

        }

        public TokenInfo TokenInfo { get; }
        public string    Namespace { get; }

    }

}