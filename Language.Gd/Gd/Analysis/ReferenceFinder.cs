using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pharmatechnik.Language.Gd {

    public class ReferenceFinder {

        readonly List<string> _refs = new();

        public IEnumerable<string> ErrorRefs {
            get { return _errorRefs; }
        }

        readonly List<string> _errorRefs = new();

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

            // TODO Hier die References auslesen.. Idealerweise über das Model, nicht den Syntaxbaum...
            var refs = _desc.DescendantNodes().OfType<ControlSectionSyntax>().Where(cs => cs.ControlSectionBegin?.ControlTypeToken.GetText() == "UserControlReference")
                            .Select(_ => new { TypeFullName = "Foo" });

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

        List<string> GetFileSuggestions(string containingGdFileName, string referenceTypeName) {

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

            var subFolders = new List<string>(subFolder.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries));

            List<string> results = new List<string>();

            List<string> foldersUmlaut1;
            StandardStrategy();
            StandardStrategy(true);
            UmlautStrategie1();
            UmlautStrategie1(true);
            UmlautStrategie2();
            UmlautStrategie2(true);
            XTPlusStrategie();
            XTPlusStrategie(true);

            //erste zwei mit '.' verbinden
            AddSuggestionsViaCombining(2, subFolders, results, startDir);
            AddSuggestionsViaCombining(2, subFolders, results, startDir, false, true);
            //"xtplus." als prefix
            AddSuggestionsViaCombining(2, subFolders, results, startDir, true);
            AddSuggestionsViaCombining(2, subFolders, results, startDir, true, true);

            //erste drei mit '.' verbinden
            AddSuggestionsViaCombining(3, subFolders, results, startDir);
            AddSuggestionsViaCombining(3, subFolders, results, startDir, false, true);
            //"xtplus." als prefix
            AddSuggestionsViaCombining(3, subFolders, results, startDir, true);
            AddSuggestionsViaCombining(3, subFolders, results, startDir, true, true);

            //erste vier mit '.' verbinden
            AddSuggestionsViaCombining(4, subFolders, results, startDir);
            AddSuggestionsViaCombining(4, subFolders, results, startDir, false, true);
            //"xtplus." als prefix
            AddSuggestionsViaCombining(4, subFolders, results, startDir, true);
            AddSuggestionsViaCombining(4, subFolders, results, startDir, true, true);

            return results;

            void StandardStrategy(bool shared = false) {
                //Standard-Strategy
                var foldersStandard = new List<string>(subFolders);
                if (shared) {
                    foldersStandard[0] += ".Shared";
                }

                AddSuggestions(foldersStandard, results, startDir);
            }

            void UmlautStrategie1(bool shared = false) {
                //Umlaute Variante1
                foldersUmlaut1 = new List<string>(subFolders);
                if (shared) {
                    foldersUmlaut1[0] += ".Shared";
                }

                for (int i = 0; i < foldersUmlaut1.Count; i++) {
                    foldersUmlaut1[i] = Converter.ConvertUmlaute(foldersUmlaut1[i]);
                }

                AddSuggestions(foldersUmlaut1, results, startDir);
            }

            void UmlautStrategie2(bool shared = false) {
                //Umlaute Variante2
                foldersUmlaut1 = new List<string>(subFolders);
                if (shared) {
                    foldersUmlaut1[0] += ".Shared";
                }

                for (int i = 0; i < foldersUmlaut1.Count; i++) {
                    foldersUmlaut1[i] = Converter.ReverseUmlaute(foldersUmlaut1[i]);
                }

                AddSuggestions(foldersUmlaut1, results, startDir);
            }

            void XTPlusStrategie(bool shared = false) {
                //XTplus. in added
                foldersUmlaut1    = new List<string>(subFolders);
                foldersUmlaut1[0] = "XTplus." + foldersUmlaut1[0];
                if (shared) {
                    foldersUmlaut1[0] += ".Shared";
                }

                AddSuggestions(foldersUmlaut1, results, startDir);
            }
        }

        private void AddSuggestionsViaCombining(int combine, List<string> subFolders, List<string> results, string startDir, bool xtplusprefix, bool shared = false) {

            if (subFolders.Count >= combine) {
                List<string> foldersCloned  = new List<string>();
                string       combinedFolder = subFolders[0];
                for (int c = 1; c < combine; c++) {
                    combinedFolder += $".{subFolders[c]}";
                }

                //combine-Anzahl zusammenfassen
                if (shared) {
                    foldersCloned.Add(combinedFolder + ".Shared");
                } else {
                    foldersCloned.Add(combinedFolder);
                }

                //nach combine-Anzahl weitermachen
                for (int i = combine; i < subFolders.Count; i++) {
                    foldersCloned.Add(subFolders[i]);
                }

                if (xtplusprefix) {
                    foldersCloned[0] = "XTplus." + foldersCloned[0];
                }

                AddSuggestions(foldersCloned, results, startDir);
            }
        }

        private void AddSuggestionsViaCombining(int combine, List<string> subFolders, List<string> results, string startDir) {
            AddSuggestionsViaCombining(combine, subFolders, results, startDir, false);
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

    internal class Converter {

        public static string ConvertUmlaute(string text) {
            return text.Replace("ä", "ae").Replace("ü", "ue").Replace("ö", "oe").Replace("Ä", "Ae").Replace("Ü", "Ue").Replace("Ö", "Oe");
        }

        public static string ReverseUmlaute(string text) {
            return text.Replace("ae", "ä").Replace("ue", "ü").Replace("oe", "ö").Replace("Ae", "Ä").Replace("Ue", "Ü").Replace("Oe", "Ö");
        }

    }

    class ImplicitPathAnalyser {

        public DirectoryInfo ProjectRootDirectory { get; }
        public DirectoryInfo UiRootDirectory      { get; }
        public DirectoryInfo IwflRootDirectory    { get; }

        public bool IsSharedProject => ProjectRootDirectory?.FullName.EndsWith(".Shared") ?? false;

        public ImplicitPathAnalyser(string fileNameOrPath) {

            DirectoryInfo dir = new DirectoryInfo(fileNameOrPath);
            while (dir.Parent != null) {

                if (dir.Parent.Name == "src") {
                    ProjectRootDirectory = dir;
                    break;
                }

                dir = dir.Parent;
            }

            if (ProjectRootDirectory == null) {
                return;
            }

            if (IsSharedProject) {
                UiRootDirectory   = new DirectoryInfo(ProjectRootDirectory.FullName.Replace(".Shared", ".Client"));
                IwflRootDirectory = ProjectRootDirectory;
            } else {
                //Default-Generierung
                ProjectRootDirectory = null;
            }

        }

    }

}