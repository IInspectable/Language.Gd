#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Tool.GdSyntaxGenerator.Models {

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    class EnumMemberModel {

        public string Name  { get; set; }
        public object Value { get; set; }

    }

}