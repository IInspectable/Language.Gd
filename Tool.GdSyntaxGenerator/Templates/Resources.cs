﻿#region Using Directives

using System.IO;

#endregion

namespace Tool.GdSyntaxGenerator.Templates {

    static class Resources {

        // ReSharper disable InconsistentNaming
        public static readonly string CommonTemplate         = LoadText("Common.stg");
        public static readonly string SyntaxKindEnumTemplate = LoadText("SyntaxKindEnum.stg");
        public static readonly string SyntaxSlotTemplate     = LoadText("SyntaxSlot.stg");
        public static readonly string SyntaxNodeTemplate     = LoadText("SyntaxNode.stg");

        // ReSharper restore InconsistentNaming

        static string LoadText(string resourceName) {

            var fullResourceName = $"{typeof(Resources).Namespace}.{resourceName}";

            using (Stream stream = typeof(Resources).Assembly.GetManifestResourceStream(fullResourceName))
                // ReSharper disable once AssignNullToNotNullAttribute Lass krachen...
            using (StreamReader reader = new StreamReader(stream)) {
                string result = reader.ReadToEnd();
                return result;
            }
        }

    }

}