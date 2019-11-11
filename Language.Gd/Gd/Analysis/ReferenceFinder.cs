using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pharmatechnik.Language.Gd {

    public class ReferenceFinder {

        List<string> _refs = new List<string>();

        public IEnumerable<string> ErrorRefs {
            get { return _errorRefs; }
        }

        List<string> _errorRefs = new List<string>();

        public IEnumerable<string> References {
            get { return _refs; }
        }

        private readonly GuiDescriptionSyntax _desc;
        private readonly string               _gdFile;

        public ReferenceFinder(GuiDescriptionSyntax desc) {
            this._desc   = desc;
            this._gdFile = desc.SyntaxTree?.SourceText?.FileInfo?.FullName;
            Parse();
        }

        private void Parse() {

            // TODO Hier die Refeences auslesen.. Idealerweise über das Model, nicht den Syntaxbaum...
            var refs = _desc.DescendantNodes().OfType<ControlSectionSyntax>().Where(cs => cs.ControlSectionBegin?.ControlTypeToken.GetText() == "UserControlReference")
                            .Select(cs => new {TypeFullName = "Foo"});

            foreach (var reference in refs) {

                // TODO wieder einbauen, wenn Model exisitiert
                //if (reference.NoGdRef) {
                //    continue;
                //}

                var r       = new Reference(_gdFile, reference.TypeFullName);
                var newFile = string.Empty;
                if (FileExists(r, ref newFile))
                    _refs.Add(newFile);
                else
                    _errorRefs.Add(newFile);
            }
        }

        public static bool TryResolveFileName(Reference r, out string fileName) {
            fileName = String.Empty;
            var newFile = string.Empty;
            if (FileExists(r, ref newFile)) {
                fileName = newFile;
                return true;
            }
            return false;
        }

        private static bool FileExists(Reference r, ref string newFile) {

            if (Try2Find(r, ref newFile, SearchOption.TopDirectoryOnly)) {
                return true;
            }

            if (Try2Find(r, ref newFile, SearchOption.AllDirectories)) {
                return true;
            }

            if (string.IsNullOrEmpty(newFile)) {
                var sb = new StringBuilder();
                sb.AppendFormat("ReferenceFinder, File not found::: Filename={0}\n", r.ReferenceFileName);
                sb.AppendFormat(" CurrentfilePathName: {0}",                         r.ContainingGdFileName);
                sb.AppendFormat(" RefFullClassName: {0}",                            r.ReferenceTypeName);
                sb.AppendFormat(" SuggestedPaths: {0}",                              string.Join("; ", r.GetSuggestedPaths()));
                newFile = sb.ToString();
            }

            return false;
        }

        private static bool Try2Find(Reference r, ref string newFile, SearchOption searchOption) {

            foreach (var suggestedPath in r.GetSuggestedPaths()) {

                if (!Directory.Exists(suggestedPath))
                    continue;

                if (FindInDirectory(suggestedPath, r.ReferenceFileName, ref newFile, searchOption))
                    return true;

                if (FindInDirectory(suggestedPath, r.ReferenceFileName.Remove(0, 3), ref newFile, searchOption))
                    return true;

                if (FindInDirectory(suggestedPath, "Usr" + r.ReferenceFileName, ref newFile, searchOption))
                    return true;

                if (FindInDirectory(suggestedPath, "Frm" + r.ReferenceFileName, ref newFile, searchOption))
                    return true;

            }

            return false;
        }

        private static bool FindInDirectory(string suggestedPath, string pattern, ref string newFile, SearchOption searchOption) {
           
            if (TryFind(pattern, searchOption, suggestedPath, ref newFile))
                return true;

            if (TryFind(UmlautConverter.ConvertUmlaute(pattern), searchOption, suggestedPath, ref newFile))
                return true;

            if (TryFind(UmlautConverter.ReverseUmlaute(pattern), searchOption, suggestedPath, ref newFile))
                return true;

            return false;
        }

        private static bool TryFind(string pattern, SearchOption searchOption, string suggestedPath, ref string newFile) {
            
            var files = Directory.GetFiles(suggestedPath, pattern, searchOption);
            
            if (files.Length == 1) {
                newFile = files[0];
                return true;
            } else if (files.Length > 1) {
                newFile = $"ReferenceFinder: Several files match: {files[0]} <--> {files[1]}";
                return false;
            }

            return false;
        }

    }

    public class Reference {

        public Reference(string containingGdFileName, string referenceTypeName) {
            ContainingGdFileName = containingGdFileName ?? String.Empty;
            ReferenceTypeName    = referenceTypeName    ?? String.Empty;
        }

        public string ContainingGdFileName { get; }
        public string ReferenceTypeName    { get; }
        public string ReferenceFileName    => ReferenceTypeName.Substring(ReferenceTypeName.LastIndexOf(".", StringComparison.Ordinal) + 1) + ".gd";

        public List<string> GetSuggestedPaths() => GetFileSuggestions(ContainingGdFileName, ReferenceTypeName);

        static List<string> GetFileSuggestions(string containingGdFileName, string referenceTypeName) {

            var subFolder = referenceTypeName.Replace("Pharmatechnik.Apotheke.XTplus", "");

            //Problem mit Xtplus.Common vs. Application.Common vs. Common.Softwareversioninfo vs. Common.Aufgabenplanung
            if (referenceTypeName.StartsWith("Pharmatechnik.Apotheke.XTplus.Common.KonfigurierbareTabellen") ||
                referenceTypeName.StartsWith("Pharmatechnik.Apotheke.XTplus.Common.DokumentScanner")         ||
                referenceTypeName.StartsWith("Pharmatechnik.Apotheke.XTplus.Common.LaenderUndWaehrungen")) {
                subFolder = ".Application" + subFolder;
            }

            var startDir = Path.GetDirectoryName(containingGdFileName);
            startDir = startDir?.Substring(0, startDir.LastIndexOf(Path.DirectorySeparatorChar + "src", StringComparison.Ordinal) + 4);

            subFolder = subFolder.Remove(subFolder.LastIndexOf(".GUI.", StringComparison.Ordinal), subFolder.Length - subFolder.LastIndexOf(".GUI.", StringComparison.Ordinal));

            var subFolders = new List<string>(subFolder.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries));

            var results = new List<string>();

            //Standard-Strategy
            var foldersCloned = new List<string>(subFolders);
            AddSuggestions(foldersCloned, results, startDir);

            //Umlaute Variante1
            foldersCloned = new List<string>(subFolders);
            for (var i = 0; i < foldersCloned.Count; i++) {
                foldersCloned[i] = UmlautConverter.ConvertUmlaute(foldersCloned[i]);

            }

            AddSuggestions(foldersCloned, results, startDir);

            //Umlaute Variante2
            foldersCloned = new List<string>(subFolders);
            for (var i = 0; i < foldersCloned.Count; i++) {
                foldersCloned[i] = UmlautConverter.ReverseUmlaute(foldersCloned[i]);

            }

            AddSuggestions(foldersCloned, results, startDir);

            //XTplus. in added
            foldersCloned    = new List<string>(subFolders);
            foldersCloned[0] = "XTplus." + foldersCloned[0];
            AddSuggestions(foldersCloned, results, startDir);

            //erste zwei mit '.' verbinden
            AddSuggestionsViaCombining(2, subFolders, results, startDir, xtplusprefix: false);
            //"xtplus." als prefix
            AddSuggestionsViaCombining(2, subFolders, results, startDir, xtplusprefix: true);

            //erste drei mit '.' verbinden
            AddSuggestionsViaCombining(3, subFolders, results, startDir, xtplusprefix: false);
            //"xtplus." als prefix
            AddSuggestionsViaCombining(3, subFolders, results, startDir, xtplusprefix: true);

            //erste vier mit '.' verbinden
            AddSuggestionsViaCombining(4, subFolders, results, startDir, xtplusprefix: false);
            //"xtplus." als prefix
            AddSuggestionsViaCombining(4, subFolders, results, startDir, xtplusprefix: true);

            return results;
        }

        static void AddSuggestionsViaCombining(
            int combine,
            List<string> subFolders,
            List<string> results, string startDir,
            bool xtplusprefix) {

            if (subFolders.Count >= combine) {
                var foldersCloned  = new List<string>();
                var combinedFolder = subFolders[0];
                for (var c = 1; c < combine; c++) {
                    combinedFolder += $".{subFolders[c]}";
                }

                //combine-Anzahl zusammenfassen
                foldersCloned.Add(combinedFolder);

                //nach combine-Anzahl weitermachen
                for (var i = combine; i < subFolders.Count; i++)
                    foldersCloned.Add(subFolders[i]);

                if (xtplusprefix) {
                    foldersCloned[0] = "XTplus." + foldersCloned[0];
                }

                AddSuggestions(foldersCloned, results, startDir);
            }
        }

        static void AddSuggestions(List<string> subFolders, List<string> results, string startDir) {

            var tempFolders = new List<string>(subFolders);

            while (tempFolders.Count > 0) {
                var suggestion = DirectorySuggestion(startDir, tempFolders);
                if (!results.Contains(suggestion)) {
                    results.Add(suggestion);
                }

                tempFolders.RemoveAt(tempFolders.Count - 1);
            }

            tempFolders = new List<string>(subFolders);
            while (tempFolders.Count > 0) {
                var suggestion = DirectorySuggestion(startDir, tempFolders);
                if (!results.Contains(suggestion)) {
                    results.Add(suggestion);
                }

                tempFolders.RemoveAt(0);
            }

        }

        static string DirectorySuggestion(string newDir, List<string> subFolders) {
            return subFolders.Aggregate(newDir, Path.Combine);
        }

    }

    static class UmlautConverter {

        public static string ConvertUmlaute(string text) {
            return text.Replace("ä", "ae").Replace("ü", "ue").Replace("ö", "oe").Replace("Ä", "Ae").Replace("Ü", "Ue").Replace("Ö", "Oe");
        }

        public static string ReverseUmlaute(string text) {
            return text.Replace("ae", "ä").Replace("ue", "ü").Replace("oe", "ö").Replace("Ae", "Ä").Replace("Ue", "Ü").Replace("Oe", "Ö");
        }

    }

}