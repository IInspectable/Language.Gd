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
            get {
                return _refs;
            }
        }
      
        private readonly GuiDescriptionSyntax _desc;
        private readonly string _gdFile;

        public ReferenceFinder(GuiDescriptionSyntax desc) {
            this._desc = desc;
            this._gdFile = desc.SyntaxTree?.SourceText?.FileInfo?.FullName;
            Parse();
        }

        private void Parse() {

            // TODO Hier die Refeences auslesen.. Idealerweise über das Model, nicht den Syntaxbaum...
            var refs=_desc.DescendantNodes().OfType<ControlSectionSyntax>().
                           Where(cs => cs.ControlSectionBegin?.ControlTypeToken.GetText()=="UserControlReference")
                          .Select(cs=> new{TypeFullName="Foo"});

            foreach (var reference in refs) {

                //if (reference.NoGdRef) {
                //    continue;
                //}

                Reference r = new Reference(_gdFile, reference.TypeFullName);
                string newFile = string.Empty;
                if (FileExists(r, ref newFile))
                    _refs.Add(newFile);
                else
                    _errorRefs.Add(newFile);
            }
        }


        private bool FileExists(Reference r, ref string newFile) {

            if (Try2Find(r, ref newFile, SearchOption.TopDirectoryOnly)) {
                return true;
            }

            if (Try2Find(r, ref newFile, SearchOption.AllDirectories)) {
                return true;
            }


            if (string.IsNullOrEmpty(newFile)) {
                var sb = new StringBuilder();
                sb.AppendFormat("ReferenceFinder, File not found::: Filename={0}\n", r.RefFileName);
                sb.AppendFormat(" CurrentfilePathName: {0}", r.CurrentfilePathName);
                sb.AppendFormat(" RefFullClassName: {0}", r.RefFullClassName);
                sb.AppendFormat(" SuggestedPaths: {0}", string.Join("; ", r.SuggestedPaths));
                newFile = sb.ToString();
            }
            return false;
        }




        private bool Try2Find(Reference r, ref string newFile, SearchOption searchOption) {
            foreach (string suggestedPath in r.SuggestedPaths) {
                if (!Directory.Exists(suggestedPath))
                    continue;

                if (FindInDirectory(suggestedPath, r.RefFileName, ref newFile, searchOption))
                    return true;

                if (FindInDirectory(suggestedPath, r.RefFileName.Remove(0, 3), ref newFile, searchOption))
                    return true;
                if (FindInDirectory(suggestedPath, "Usr" + r.RefFileName, ref newFile, searchOption))
                    return true;

                if (FindInDirectory(suggestedPath, "Frm" + r.RefFileName, ref newFile, searchOption))
                    return true;

            }
            return false;
        }

        private bool FindInDirectory(string suggestedPath, string pattern, ref string newFile, SearchOption searchOption) {
            if (TryFind(pattern, searchOption, suggestedPath, ref newFile))
                return true;

            if (TryFind(Converter.ConvertUmlaute(pattern), searchOption, suggestedPath, ref newFile))
                return true;

            if (TryFind(Converter.ReverseUmlaute(pattern), searchOption, suggestedPath, ref newFile))
                return true;

            return false;
        }

        private bool TryFind(string pattern, SearchOption searchOption, string suggestedPath, ref string newFile) {
            string[] files = Directory.GetFiles(suggestedPath, pattern, searchOption);
            if (files.Length == 1) {
                newFile = files[0];
                return true;
            }
            else if (files.Length > 1) {
                newFile = string.Format("ReferenceFinder: Several files match: {0} <--> {1}", files[0], files[1]);
                return false;
            }

            return false;
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

    public class Reference {
        private readonly string _currentfilePathName;
        private readonly string _refFullClassName;

        public Reference(string currentfilePathName, string refFullClassName) {
            this._currentfilePathName = currentfilePathName;
            this._refFullClassName = refFullClassName;
        }


        public string RefFileName {
            get {
                return _refFullClassName.Substring(_refFullClassName.LastIndexOf(".") + 1) + ".gd";
            }
        }

        public string CurrentfilePathName {
            get { return _currentfilePathName; }
        }

        public string RefFullClassName {
            get { return _refFullClassName; }
        }

        public List<string> SuggestedPaths {
            get {
                string subFolder;
                string startDir;


                subFolder = _refFullClassName.Replace("Pharmatechnik.Apotheke.XTplus", "");
                
                //Problem mit Xtplus.Common vs. Application.Common vs. Common.Softwareversioninfo vs. Common.Aufgabenplanung
                if (_refFullClassName.StartsWith("Pharmatechnik.Apotheke.XTplus.Common.KonfigurierbareTabellen") ||
                    _refFullClassName.StartsWith("Pharmatechnik.Apotheke.XTplus.Common.DokumentScanner") ||
                    _refFullClassName.StartsWith("Pharmatechnik.Apotheke.XTplus.Common.LaenderUndWaehrungen")) {
                    subFolder = ".Application" + subFolder;
                }
                startDir = Path.GetDirectoryName(_currentfilePathName);
                startDir = startDir.Substring(0, startDir.LastIndexOf(Path.DirectorySeparatorChar + "src") + 4);

                subFolder =
                            subFolder.Remove(subFolder.LastIndexOf(".GUI."), subFolder.Length - subFolder.LastIndexOf(".GUI."));
                char[] split = new[] { '.' };

                List<string> subFolders = new List<string>(
                            subFolder.Split(split, StringSplitOptions.RemoveEmptyEntries));

                List<string> results = new List<string>();

                //Standard-Strategy
                List<string> foldersCloned = new List<string>(subFolders);
                AddSuggestions(foldersCloned, results, startDir);

                //Umlaute Variante1
                foldersCloned = new List<string>(subFolders);
                for (int i = 0; i < foldersCloned.Count; i++) {
                    foldersCloned[i] = Converter.ConvertUmlaute(foldersCloned[i]);

                }
                AddSuggestions(foldersCloned, results, startDir);


                //Umlaute Variante2
                foldersCloned = new List<string>(subFolders);
                for (int i = 0; i < foldersCloned.Count; i++) {
                    foldersCloned[i] = Converter.ReverseUmlaute(foldersCloned[i]);

                }
                AddSuggestions(foldersCloned, results, startDir);

                //XTplus. in added
                foldersCloned = new List<string>(subFolders);
                foldersCloned[0] = "XTplus." + foldersCloned[0];
                AddSuggestions(foldersCloned, results, startDir);
                
                
                
                //erste zwei mit '.' verbinden
                AddSuggestionsViaCombining(2, subFolders, results, startDir);
                //"xtplus." als prefix
                AddSuggestionsViaCombining(2, subFolders, results, startDir,true);
               

                //erste drei mit '.' verbinden
                AddSuggestionsViaCombining(3, subFolders, results, startDir);
                //"xtplus." als prefix
                AddSuggestionsViaCombining(3, subFolders, results, startDir, true);
               
                //erste vier mit '.' verbinden
                AddSuggestionsViaCombining(4, subFolders, results, startDir);
                //"xtplus." als prefix
                AddSuggestionsViaCombining(4, subFolders, results, startDir, true);

                return results;
            }

        }

        private void AddSuggestionsViaCombining(int combine, List<string> subFolders, List<string> results, string startDir, bool xtplusprefix) {

            if (subFolders.Count >= combine) {
                List<string> foldersCloned = new List<string>();
                string combinedFolder = subFolders[0];
                for (int c = 1; c < combine; c++) {
                    combinedFolder += string.Format(".{0}", subFolders[c]);
                }
                //combine-Anzahl zusammenfassen
                foldersCloned.Add(combinedFolder);

                //nach combine-Anzahl weitermachen
                for (int i = combine; i < subFolders.Count; i++)
                    foldersCloned.Add(subFolders[i]);

                if (xtplusprefix)
                    foldersCloned[0] = "XTplus." + foldersCloned[0];
                
                AddSuggestions(foldersCloned, results, startDir);
            }
        }

        private void AddSuggestionsViaCombining(int combine, List<string> subFolders, List<string> results, string startDir) {
            AddSuggestionsViaCombining(combine, subFolders, results, startDir, false);
        }
        private void AddSuggestions(List<string> subFolders, List<string> results, string startDir) {
            List<string> tempFolders = new List<string>(subFolders);

            while (tempFolders.Count > 0) {
                string suggestion = DirectorySuggestion(startDir, tempFolders);
                if (!results.Contains(suggestion))
                    results.Add(suggestion);
                tempFolders.RemoveAt(tempFolders.Count - 1);
            }

            tempFolders = new List<string>(subFolders);
            while (tempFolders.Count > 0) {
                string suggestion = DirectorySuggestion(startDir, tempFolders);
                if (!results.Contains(suggestion))
                    results.Add(suggestion);
                tempFolders.RemoveAt(0);
            }

        }

        private string DirectorySuggestion(string newDir, List<string> subFolders) {

            foreach (string part in subFolders) {
                newDir = Path.Combine(newDir, part);
            }

            return newDir;
        }
    }

}
