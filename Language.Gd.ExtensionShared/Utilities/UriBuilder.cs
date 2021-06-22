#region Using Directives

using System;
using System.IO;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Utilities {

    static class UriBuilder {

        [CanBeNull]
        public static Uri BuildDirectoryUriFromDirectory(string directory) {

            if (String.IsNullOrEmpty(directory)) {
                return null;
            }

            // Nur wenn der Pfad mit einem \ oder / endet, ist sichergestellt, dass der Uri als Verzeichnis und nicht
            // als Datei erkannt wird. Sobald nämlich der Pfad einen Punkt im letzten Verzeichnis enthält, wird dieser als Datei gesehen
            // Beispiel: "c:\ws\Nav.Project" wird als Datei gesehen, "c:\ws\Nav.Project\" dagegen als Verzeichnis
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !directory.EndsWith(Path.AltDirectorySeparatorChar.ToString())) {

                directory += Path.DirectorySeparatorChar;
            }

            return new Uri(directory);

        }

        [CanBeNull]
        public static Uri BuildDirectoryUriFromFile(string fileName) {
            if (String.IsNullOrEmpty(fileName)) {
                return null;
            }

            return BuildDirectoryUriFromDirectory(Path.GetDirectoryName(fileName));
        }

    }

}