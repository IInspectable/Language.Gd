using JetBrains.Annotations;

namespace Pharmatechnik.Language.Gd {

    public interface ISectionSyntax {

        [CanBeNull]
        SyntaxNode SectionBegin { get; }

        [CanBeNull]
        SyntaxNode SectionEnd { get; }

    }

    public interface INamedSectionSyntax: ISectionSyntax {

        SyntaxToken? NameBegin { get; }
        SyntaxToken? NameEnd   { get; }

    }

    // TODO Nur zum Testen...

    partial class NamespaceDeclarationSectionSyntax: ISectionSyntax {

        SyntaxNode ISectionSyntax.SectionBegin => NamespaceDeclarationSectionBegin;
        SyntaxNode ISectionSyntax.SectionEnd   => NamespaceDeclarationSectionEnd;

    }

    partial class HotkeysSectionSyntax: ISectionSyntax {

        SyntaxNode ISectionSyntax.SectionBegin => HotkeysSectionBegin;
        SyntaxNode ISectionSyntax.SectionEnd   => HotkeysSectionEnd;

    }

    partial class PropertiesSectionSyntax: ISectionSyntax {

        SyntaxNode ISectionSyntax.SectionBegin => PropertiesSectionBegin;
        SyntaxNode ISectionSyntax.SectionEnd   => PropertiesSectionEnd;

    }

    partial class ControlSectionSyntax: INamedSectionSyntax {

        SyntaxNode ISectionSyntax.SectionBegin => ControlSectionBegin;
        SyntaxNode ISectionSyntax.SectionEnd   => ControlSectionEnd;

        SyntaxToken? INamedSectionSyntax.NameBegin => ControlSectionBegin?.ControlNameToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => ControlSectionEnd?.ControlNameToken;

    }

    partial class FormSectionSyntax: INamedSectionSyntax {

        SyntaxNode ISectionSyntax.SectionBegin => FormSectionBegin;
        SyntaxNode ISectionSyntax.SectionEnd   => FormSectionEnd;

        SyntaxToken? INamedSectionSyntax.NameBegin => FormSectionBegin?.FormToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => FormSectionEnd?.FormToken;

    }

}