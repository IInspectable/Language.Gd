using JetBrains.Annotations;

namespace Pharmatechnik.Language.Gd.QuickInfo {

    public static class QuickInfoProvider {

        // TODO GetQuickInfoDefinition
        [CanBeNull]
        public static QuickInfoDefinition GetQuickInfoDefinition(SyntaxTree syntaxTree, int triggerPoint) {

            var triggerToken = syntaxTree.Root.FindToken(triggerPoint, excludeTrivia: true);

            if (triggerToken.IsMissing || triggerToken.Parent == null) {
                return null;
            }

            // Named Sections
            if (triggerToken.Parent?.Parent is INamedSectionSyntax namedSection &&
                namedSection.SectionBegin != null                               &&
                (namedSection.NameBegin  == triggerToken ||
                 namedSection.SectionEnd == triggerToken.Parent)) {

                var glyph   = SectionGlyphs.GetGlyph(namedSection as SyntaxNode);
                var content = namedSection.SectionBegin.ToSimplifiedText();

                return new QuickInfoDefinition(triggerToken.Extent, glyph, content);
            }

            // TODO elementAccessExpression berücksichtigen bzw. was will man zeigen?
            if (triggerToken.Parent?.Parent?.Parent is LvalueExpressionSyntax lvalueExpression) {

                // Properties
                if (lvalueExpression.Parent is PropertyAssignSyntax) {
                    var glyph   = Glyph.Property;
                    var content = lvalueExpression.ToSimplifiedText();

                    return new QuickInfoDefinition(lvalueExpression.Extent, glyph, content);
                }

                // Events in Properties
                if (lvalueExpression.Parent is PropertyAddAssignSyntax) {
                    var glyph   = Glyph.Event;
                    var content = lvalueExpression.ToSimplifiedText();

                    return new QuickInfoDefinition(lvalueExpression.Extent, glyph, content);
                }

            }

            // Events in EventDeclaration
            if (triggerToken.Parent is EventDeclarationSyntax eventDeclaration &&
                eventDeclaration.EventNameToken == triggerToken) {

                var glyph   = Glyph.Event;
                var content = triggerToken.ToSimplifiedText();

                return new QuickInfoDefinition(triggerToken.Extent, glyph, content);
            }

            // Namespace
            {
                // NAMESPACE Foo.Bar.Baz
                //           ^----------^- Nur dieser Bereich triggert den Tooltip
                if (triggerToken.Parent.HasAncestorOfType<NamespaceDeclarationSectionBeginSyntax>(out var namespaceSectionBegin) &&
                    triggerToken.Parent.HasAncestorOfType<QualifiedNameSyntax>()) {

                    var glyph   = SectionGlyphs.GetGlyph(namespaceSectionBegin);
                    var content = namespaceSectionBegin.ToSimplifiedText();

                    return new QuickInfoDefinition(triggerToken.Extent, glyph, content);
                }
            }
            {
                // END NAMESPACE
                if (triggerToken.Parent is NamespaceDeclarationSectionEndSyntax namespaceSectionEnd      &&
                    namespaceSectionEnd.Parent is NamespaceDeclarationSectionSyntax namespaceDeclaration &&
                    namespaceDeclaration.SectionBegin is NamespaceDeclarationSectionBeginSyntax namespaceSectionBegin) {

                    var glyph   = SectionGlyphs.GetGlyph(namespaceDeclaration);
                    var content = namespaceSectionBegin.ToSimplifiedText();

                    return new QuickInfoDefinition(triggerToken.Extent, glyph, content);
                }
            }

            return null;
        }

    }

}