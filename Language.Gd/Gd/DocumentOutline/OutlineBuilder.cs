#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.DocumentOutline {

    public class OutlineBuilder: SyntaxVisitor<OutlineElement> {

        [CanBeNull]
        public static OutlineElement Build(SyntaxNode node) {
            return Build(node as GuiDescriptionSyntax);
        }

        [CanBeNull]
        public static OutlineElement Build(GuiDescriptionSyntax syntaxRoot) {

            if (syntaxRoot == null) {
                return null;
            }

            var builder = new OutlineBuilder();

            // Startpunkt ist bisweilen immer der Container
            var container = syntaxRoot.DescendantNodes().OfType<ContainerSyntax>().FirstOrDefault();
            if (container == null) {
                return null;
            }

            return builder.Visit(container);
        }

        protected internal override OutlineElement VisitContainerSyntax(ContainerSyntax container) {

            var section = container as IContainerSyntax;

            return CreateSectionWithChildElements(container,
                                                  section?.ContainerDeclaration?.ControlsSection?.GuiElements,
                                                  section?.ContainerDeclaration?.NonVisualControlsSection?.ControlSections);
        }

        protected internal override OutlineElement VisitPanelSectionSyntax(PanelSectionSyntax panelSection) {
            return CreateSectionWithChildElements(panelSection, panelSection?.ControlsSection?.GuiElements);
        }

        protected internal override OutlineElement VisitDetailsPanelSectionSyntax(DetailsPanelSectionSyntax detailsPanelSection) {
            return CreateSectionWithChildElements(detailsPanelSection, detailsPanelSection.ControlsSection?.GuiElements);
        }

        protected internal override OutlineElement VisitGuiElementSyntax(GuiElementSyntax guiElement) {
            return CreateSectionElement(guiElement);
        }

        protected internal override OutlineElement VisitBarManagerSectionSyntax(BarManagerSectionSyntax barManagerSection) {
            return CreateSectionWithChildElements(barManagerSection, barManagerSection.ControlsSection?.GuiElements);
        }

        protected internal override OutlineElement VisitTabPageSectionSyntax(TabPageSectionSyntax tabPageSection) {
            return CreateSectionWithChildElements(tabPageSection, tabPageSection.ControlsSection?.GuiElements);
        }

        protected internal override OutlineElement VisitTabNavigationSectionSyntax(TabNavigationSectionSyntax tabNavigationSection) {
            return CreateSectionWithChildElements(tabNavigationSection,
                                                  tabNavigationSection.TabsSection?.TabPageSections,
                                                  tabNavigationSection.SharedControlSection?.ControlSections);
        }

        protected internal override OutlineElement VisitUserControlsSectionSyntax(UserControlsSectionSyntax userControlsSection) {
            return CreateSectionWithChildElements(userControlsSection, userControlsSection.ControlSections);
        }

        protected internal override OutlineElement VisitNonVisualControlsSectionSyntax(NonVisualControlsSectionSyntax nonVisualControlsSection) {
            return CreateSectionWithChildElements(nonVisualControlsSection, nonVisualControlsSection.ControlSections);
        }

        protected internal override OutlineElement VisitMultiViewSectionSyntax(MultiViewSectionSyntax multiViewSection) {
            return CreateSectionWithChildElements(multiViewSection, multiViewSection.UserControlsSection?.ControlSections);
        }

        [CanBeNull]
        OutlineElement CreateSectionWithChildElements(SyntaxNode sectionSyntax, params IEnumerable<SyntaxNode>[] childLists) {

            var children = ImmutableArray.CreateBuilder<OutlineElement>();

            foreach (var childElements in childLists) {

                if (childElements != null) {
                    foreach (var childElement in childElements) {
                        var child = Visit(childElement);
                        if (child != null) {
                            children.Add(child);
                        }
                    }
                }
            }

            return CreateSectionElement(sectionSyntax, children.ToImmutable());
        }

        [CanBeNull]
        OutlineElement CreateSectionElement(SyntaxNode sectionSyntax, ImmutableArray<OutlineElement> children = default) {

            var section      = sectionSyntax as ISectionSyntax;
            var sectionBegin = section?.SectionBegin;
            var sectionEnd   = section?.SectionEnd;

            if (sectionBegin == null || sectionBegin.IsMissing ||
                sectionEnd   == null || sectionEnd.IsMissing) {
                return null;
            }

            var displayName = GetDisplayParts(sectionBegin);
            if (!displayName.Any()) {
                return null;
            }

            var glyph = SectionGlyphs.GetGlyph(sectionSyntax);

            return new OutlineElement(displayName, sectionSyntax.FullExtent, sectionSyntax.ExtentStart, glyph, children);
        }

        ImmutableArray<ClassifiedText> GetDisplayParts(SyntaxNode node) {

            if (node == null) {
                return ImmutableArray<ClassifiedText>.Empty;
            }

            var prefixParts = TryGetPrefixParts(node);
            var textParts = node.ToSimplifiedText()
                                 // CONTROL Keyword überspringen
                                .SkipWhile(ct => ct.Text == SyntaxFacts.Control)
                                 // Skip Leading Whitespace
                                .SkipWhile(p => p == WhiteSpace)
                                 // Damit es nicht "zu bunt" wird..
                                .Select(ct => ct.WithClassification(GdClassification.Text));

            var displayParts = prefixParts.Join(textParts, WhiteSpace);

            return displayParts.ToImmutableArray();
        }

        IReadOnlyCollection<ClassifiedText> TryGetPrefixParts(SyntaxNode node) {

            IReadOnlyCollection<ClassifiedText> result = null;
            if (node is ControlSectionBeginSyntax controlBeginSection &&
                controlBeginSection.Parent is ControlSectionSyntax controlSection) {

                if (controlSection.HotkeysSection is HotkeysSectionSyntax hotkeysSection) {

                    result = hotkeysSection.HotkeyDeclarations
                                           .Select(GetBindingParts)
                                           .FirstOrDefault(bs => bs.Count > 0);
                }

                // TODO: Entweder via Visitor abfrühstücken, oder per Semantic Model
                var textCandidate = controlBeginSection.ControlTypeToken.Text == "Label"                 ||
                                    controlBeginSection.ControlTypeToken.Text == "DynamicFunctionButton" ||
                                    controlBeginSection.ControlTypeToken.Text == "FunctionButton"        ||
                                    controlBeginSection.ControlTypeToken.Text == "Button";

                if (textCandidate &&
                    controlSection.PropertiesSection is PropertiesSectionSyntax propertiesSection) {

                    var text = propertiesSection.Propertys.OfType<PropertyAssignSyntax>()
                                                .Where(pa => pa.Rvalue is StringValueSyntax)
                                                .FirstOrDefault(p => p.LvalueExpression?.GetText() == "Text")
                                               ?.Rvalue
                                               ?.GetText()
                                               ?.PrepareNewline();

                    if (text != null) {

                        var textParts = new[] {new ClassifiedText(text, GdClassification.StringLiteral)};
                        if (result != null) {
                            return result.Join(textParts, WhiteSpace);
                        }

                        return textParts;
                    }

                }

            }

            return result ?? ImmutableArray<ClassifiedText>.Empty;
        }

        static IReadOnlyCollection<ClassifiedText> GetBindingParts(HotkeyDeclarationSyntax hotkeyDeclarationSyntax) {

            var hotKeyText = hotkeyDeclarationSyntax?.HotKeyNameToken.Text;
            if (hotKeyText.IsNullOrWhiteSpace()) {
                return ImmutableArray<ClassifiedText>.Empty;
            }

            // TODO Was ist mit den -ALT, -CTRL, etc. Modifizierern
            var modifierParts = hotkeyDeclarationSyntax.ModifierOptions
                                                       .Select(mo => mo.Modifier?.GetText().Replace("+", ""))
                                                       .Where(t => !t.IsNullOrWhiteSpace())
                                                       .Select(s => s.ToLower().ToPascalcase())
                                                       .Select(MakeHotkeyText)
                                                       .Join(ModifierSeparator);

            var hotKey = MakeHotkeyText(hotKeyText);

            return modifierParts.Join(hotKey, ModifierSeparator);
        }

        static readonly ClassifiedText WhiteSpace        = new ClassifiedText(" ", GdClassification.WhiteSpace);
        static readonly ClassifiedText ModifierSeparator = new ClassifiedText("+", GdClassification.Punctuation);

        static ClassifiedText MakeHotkeyText(string text) => new ClassifiedText(text, GdClassification.StaticSymbol);

    }

    static class OutlineBuilderExtensions {

        public static string PrepareNewline(this string text) {
            const string nl = "\u21a9";
            return text?.Replace("\\r\\n", nl)
                        .Replace("\\n", nl);
        }

        public static IReadOnlyCollection<T> Join<T>(this IEnumerable<T> source, T separator) {

            var result = new List<T>();
            using (var e = source.GetEnumerator()) {
                if (e.MoveNext()) {
                    result.Add(e.Current);
                    while (e.MoveNext()) {
                        result.Add(separator);
                        result.Add(e.Current);
                    }
                }
            }

            return result;
        }

        public static IReadOnlyCollection<T> Join<T>(this IEnumerable<T> source, T elem, T separator) {

            var result = new List<T>();
            using (var e = source.GetEnumerator()) {
                if (e.MoveNext()) {
                    result.Add(e.Current);
                    while (e.MoveNext()) {
                        result.Add(separator);
                        result.Add(e.Current);
                    }

                    result.Add(separator);
                }

                result.Add(elem);
            }

            return result;
        }

        public static IReadOnlyCollection<T> Join<T>(this IEnumerable<T> source, IEnumerable<T> elems, T separator) {

            var result = new List<T>(source);

            using (var e = elems.GetEnumerator()) {
                if (e.MoveNext()) {

                    if (result.Any()) {
                        result.Add(separator);
                    }

                    result.Add(e.Current);
                    while (e.MoveNext()) {
                        result.Add(e.Current);
                    }
                }
            }

            return result;
        }

        public static IReadOnlyCollection<T> Join<T>(this T elem, IEnumerable<T> source, T separator) {

            var result = new List<T> {
                elem
            };

            using (var e = source.GetEnumerator()) {

                while (e.MoveNext()) {
                    result.Add(separator);
                    result.Add(e.Current);
                }
            }

            return result;
        }

        public static IReadOnlyCollection<T> RemoveConsecutiveDuplicates<T>(this IEnumerable<T> source) where T : IEquatable<T> {
            var results = new List<T>();
            foreach (var element in source) {
                if (results.Count == 0 || !results.Last().Equals(element))
                    results.Add(element);
            }

            return results;
        }

    }

}