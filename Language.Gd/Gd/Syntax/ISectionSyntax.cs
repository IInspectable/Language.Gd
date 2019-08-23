namespace Pharmatechnik.Language.Gd {

    public interface ISectionSyntax {

        SyntaxNode SectionBegin { get; }
        SyntaxNode SectionEnd   { get; }

    }

    public interface INamedSectionSyntax: ISectionSyntax {

        SyntaxToken? NameBegin { get; }
        SyntaxToken? NameEnd   { get; }

    }

    // TODO Nur zum Testen...
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

}