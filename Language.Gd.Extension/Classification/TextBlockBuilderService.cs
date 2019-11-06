#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

using JetBrains.Annotations;

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Classification;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Classification {

    [Export]
    public sealed class TextBlockBuilderService {

        readonly IClassificationFormatMapService                            _classificationFormatMapService;
        readonly ImmutableDictionary<GdClassification, IClassificationType> _classificationMap;

        [ImportingConstructor]
        public TextBlockBuilderService(IClassificationFormatMapService classificationFormatMapService,
                                       IClassificationTypeRegistryService classificationTypeRegistryService) {

            _classificationFormatMapService = classificationFormatMapService;
            _classificationMap              = ClassificationTypeDefinitions.GetClassificationMap(classificationTypeRegistryService);

        }

        public IClassificationFormatMap ClassificationFormatMap => _classificationFormatMapService.GetClassificationFormatMap("tooltip");

        [CanBeNull]
        public TextBlock ToTextBlock(IReadOnlyCollection<ClassifiedText> parts) {
            ThreadHelper.ThrowIfNotOnUIThread();

            return ToTextBlock(parts, null, out _);
        }

        [CanBeNull]
        public TextBlock ToTextBlock(IReadOnlyCollection<ClassifiedText> parts, [CanBeNull] Regex searchPattern, out bool hasMatch) {

            ThreadHelper.ThrowIfNotOnUIThread();

            hasMatch = false;

            if (parts.Count == 0) {
                return null;
            }

            var runInfos  = ToRunInfo(parts, searchPattern, out hasMatch);
            var textBlock = new TextBlock {TextWrapping = TextWrapping.Wrap};

            textBlock.SetDefaultTextProperties(ClassificationFormatMap);

            var highlightedSpanBrush = hasMatch ? GetHighlightedSpanBrush() : Brushes.Transparent;
            foreach (var runInfo in runInfos) {
                var inline = ToInline(runInfo, ClassificationFormatMap, highlightedSpanBrush);
                textBlock.Inlines.Add(inline);
            }

            return textBlock;
        }

        IList<RunInfo> ToRunInfo(IReadOnlyCollection<ClassifiedText> parts, [CanBeNull] Regex searchPattern, out bool hasMatches) {
            hasMatches = false;

            if (searchPattern == null) {
                return parts.Select(part => new RunInfo(part, isMatch: false)).ToList();
            }

            var runInfos = new List<RunInfo>();
            foreach (var part in parts) {

                var matches = searchPattern.Matches(part.Text);
                if (matches.Count > 0) {

                    var currentIndex = 0;
                    foreach (Match match in matches) {

                        // Der Text vor dem Treffertext
                        if (match.Index > currentIndex) {
                            var text = part.Text.Substring(currentIndex, length: match.Index - currentIndex);
                            runInfos.Add(new RunInfo(new ClassifiedText(text, part.Classification), isMatch: false));
                        }

                        // Der Treffertext
                        runInfos.Add(new RunInfo(new ClassifiedText(match.Value, part.Classification), isMatch: true));
                        currentIndex = match.Index + match.Length;

                    }

                    // Der Text nach dem letzten Treffertext
                    if (currentIndex < part.Text.Length) {
                        var text = part.Text.Substring(currentIndex, length: part.Text.Length - currentIndex);
                        runInfos.Add(new RunInfo(new ClassifiedText(text, part.Classification), isMatch: false));
                    }

                    hasMatches = true;
                } else {
                    runInfos.Add(new RunInfo(part, false));
                }
            }

            return runInfos;
        }

        Run ToInline(RunInfo runInfo, IClassificationFormatMap formatMap, Brush highlightedSpanBrush) {

            var inline = new Run(runInfo.Text);

            _classificationMap.TryGetValue(runInfo.Classification, out var ct);
            if (ct != null) {
                var props = formatMap.GetTextProperties(ct);
                inline.SetTextProperties(props);

                if (runInfo.IsMatch) {
                    inline.Background = highlightedSpanBrush;
                }
            }

            return inline;
        }

        private static SolidColorBrush GetHighlightedSpanBrush() {

            ThreadHelper.ThrowIfNotOnUIThread();

            var uiShell5 = GdLanguagePackage.ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell5;
            var color    = uiShell5?.GetThemedWPFColor(TreeViewColors.HighlightedSpanColorKey) ?? Colors.Orange;

            return new SolidColorBrush(color);
        }

        struct RunInfo {

            readonly ClassifiedText _classifiedText;

            public RunInfo(ClassifiedText classifiedText, bool isMatch) {
                _classifiedText = classifiedText;
                IsMatch         = isMatch;
            }

            public string           Text           => _classifiedText.Text;
            public GdClassification Classification => _classifiedText.Classification;
            public bool             IsMatch        { get; }

        }

    }

}