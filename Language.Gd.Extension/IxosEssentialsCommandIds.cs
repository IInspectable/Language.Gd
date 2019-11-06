using System;
using System.ComponentModel.Design;

namespace Pharmatechnik.Language.Gd.Extension {

    static class IxosEssentialsCommandIds {

        const int GdGenerateSelectionCommand = 0x0252;
        const int GdPreviewSelectionCommand  = 0x0253;

        public const  string GuidIXOSEssentialsPackageString = "c1d9821b-a441-4fa5-a119-30795407c9f4";
        public static Guid   GuidIXOSEssentialsPackage       = new Guid(GuidIXOSEssentialsPackageString);

        const           string GuidIXOSEssentialsPackageCmdSetString = "6b794c4c-4923-45f3-a677-8cfca59df62f";
        static readonly Guid   GuidIXOSEssentialsPackageCmdSet       = new Guid(GuidIXOSEssentialsPackageCmdSetString);

        public static CommandID GdGenerateCommandId => new CommandID(GuidIXOSEssentialsPackageCmdSet, GdGenerateSelectionCommand);
        public static CommandID GuiPreviewCommandId => new CommandID(GuidIXOSEssentialsPackageCmdSet, GdPreviewSelectionCommand);

    }

}