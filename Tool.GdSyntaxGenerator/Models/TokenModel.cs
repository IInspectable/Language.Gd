#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Tool.GdSyntaxGenerator.Models {

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    class TokenModel {

        public TokenModel(TokenInfo tokenInfo, string baseNamespace) {
            TokenInfo = tokenInfo;
            Namespace = baseNamespace;

        }

        public TokenInfo TokenInfo { get; }
        public string    Namespace { get; }

    }

}