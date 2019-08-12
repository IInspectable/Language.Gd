#region Using Directives

using System.IO;

#endregion

namespace Tool.GdSyntaxGenerator.Grammar {

    static class Resources {

        // ReSharper disable InconsistentNaming
        public static readonly string GdGrammar = LoadText("GdGrammar.g4");
        public static readonly string GdTokens  = LoadText("GdTokens.g4");

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