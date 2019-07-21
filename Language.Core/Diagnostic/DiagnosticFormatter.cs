#region Using Directives

using System;

using JetBrains.Annotations;

using Pharmatechnik.Language.IO;

#endregion

namespace Pharmatechnik.Language {

    public class DiagnosticFormatter {

        public DiagnosticFormatter(bool displayEndLocations, [CanBeNull] string workingDirectory = null) {
            DisplayEndLocations = displayEndLocations;
            WorkingDirectory    = workingDirectory;
        }
        
        public static readonly DiagnosticFormatter Instance = new DiagnosticFormatter(displayEndLocations: false, workingDirectory: null);

        public bool DisplayEndLocations { get; }
        [CanBeNull]
        public string WorkingDirectory { get; }

        public virtual string Format(Diagnostic diagnostic, IFormatProvider formatter = null) {

            if (diagnostic == null) {
                throw new ArgumentNullException(nameof(diagnostic));
            }

            // ReSharper disable once UseStringInterpolation
            return String.Format("{0}{1}: {2}: {3}",
                FormatFilePath(diagnostic, formatter),
                FormatSpan(diagnostic, formatter),
                FormatCategoryAndCode(diagnostic, formatter),
                FormatMessage(diagnostic, formatter)
                );
        }

        protected virtual string FormatFilePath(Diagnostic diagnostic, IFormatProvider formatter) {

            if (diagnostic.Location.FilePath == null) {
                return String.Empty;
            }

            if (WorkingDirectory == null) {
                return diagnostic.Location.FilePath;
            }

            return PathHelper.GetRelativePath(WorkingDirectory, diagnostic.Location.FilePath);
        }

        protected virtual string FormatSpan(Diagnostic diagnostic, IFormatProvider formatter) {
            
            var location = diagnostic.Location;

            if (DisplayEndLocations) {
                return string.Format(formatter, "({0},{1},{2},{3})",
                    location.StartLine      + 1,
                    location.StartCharacter + 1,
                    location.EndLine        + 1,
                    location.EndCharacter   + 1);
            } else {
                return String.Format(formatter, "({0},{1})",
                    location.StartLine      + 1,
                    location.StartCharacter + 1);
            }
        }

        protected virtual string FormatCategoryAndCode(Diagnostic diagnostic, IFormatProvider formatter) {
            // ReSharper disable once UseStringInterpolation
            return String.Format("{0} {1}",                
                diagnostic.Severity.ToString().ToLower(),
                diagnostic.Descriptor.Id
                );
        }

        protected virtual string FormatMessage(Diagnostic diagnostic, IFormatProvider formatter) {
            return diagnostic.Message;
        }
    }
}